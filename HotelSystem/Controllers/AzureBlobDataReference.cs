

// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client
//
// Also, add a reference to Microsoft.WindowsAzure.Storage.dll for reading from and writing to the Azure blob storage

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace HotelSystem.Controllers
{
    public class AzureBlobDataReference
    {
        // Storage connection string used for regular blobs. It has the following format:
        // DefaultEndpointsProtocol=https;AccountName=ACCOUNT_NAME;AccountKey=ACCOUNT_KEY
        // It's not used for shared access signature blobs.
        public string ConnectionString { get; set; }

        // Relative uri for the blob, used for regular blobs as well as shared access 
        // signature blobs.
        public string RelativeLocation { get; set; }

        // Base url, only used for shared access signature blobs.
        public string BaseLocation { get; set; }

        // Shared access signature, only used for shared access signature blobs.
        public string SasBlobToken { get; set; }


        public enum BatchScoreStatusCode
        {
            NotStarted,
            Running,
            Failed,
            Cancelled,
            Finished
        }

        public class BatchScoreStatus
        {
            // Status code for the batch scoring job
            public BatchScoreStatusCode StatusCode { get; set; }


            // Locations for the potential multiple batch scoring outputs
            public IDictionary<string, AzureBlobDataReference> Results { get; set; }

            // Error details, if any
            public string Details { get; set; }
        }

        public class BatchExecutionRequest
        {

            public IDictionary<string, AzureBlobDataReference> Inputs { get; set; }
            public IDictionary<string, string> GlobalParameters { get; set; }

            // Locations for the potential multiple batch scoring outputs
            public IDictionary<string, AzureBlobDataReference> Outputs { get; set; }
        }


        public static void WriteFailedResponse(HttpResponseMessage response)
        {
            Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

            // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
            Console.WriteLine(response.Headers.ToString());

            string responseContent = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseContent);
        }


        public static void SaveBlobToFile(AzureBlobDataReference blobLocation, string resultsLabel)
        {
            
            const string OutputFileLocation = "C:\\Users\\User\\Documents\\visual studio 2015\\Projects\\HotelSystem\\HotelSystem\\Content\\result.csv"; // Replace this with the location you would like to use for your output file

            var credentials = new StorageCredentials(blobLocation.SasBlobToken);
            var blobUrl = new Uri(new Uri(blobLocation.BaseLocation), blobLocation.RelativeLocation);
            var cloudBlob = new CloudBlockBlob(blobUrl, credentials);

            Console.WriteLine(string.Format("Reading the result from {0}", blobUrl.ToString()));
            cloudBlob.DownloadToFile(OutputFileLocation, FileMode.Create);

            Console.WriteLine(string.Format("{0} have been written to the file {1}", resultsLabel, OutputFileLocation));
        }



        public static void UploadFileToBlob(string inputFileLocation, string inputBlobName, string storageContainerName, string storageConnectionString)
        {
            // Make sure the file exists
            if (!File.Exists(inputFileLocation))
            {
                throw new FileNotFoundException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "File {0} doesn't exist on local computer.",
                        inputFileLocation));
            }

            Console.WriteLine("Uploading the input to blob storage...");

            var blobClient = CloudStorageAccount.Parse(storageConnectionString).CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(storageContainerName);
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(inputBlobName);
            blob.UploadFromFile(inputFileLocation, FileMode.Open);
        }



        public static string ProcessResults(BatchScoreStatus status)
        {


            bool first = true;
            foreach (var output in status.Results)
            {
                var blobLocation = output.Value;
                Console.WriteLine(string.Format("The result '{0}' is available at the following Azure Storage location:", output.Key));
                Console.WriteLine(string.Format("BaseLocation: {0}", blobLocation.BaseLocation));
                Console.WriteLine(string.Format("RelativeLocation: {0}", blobLocation.RelativeLocation));
                Console.WriteLine(string.Format("SasBlobToken: {0}", blobLocation.SasBlobToken));
                Console.WriteLine();


                // Save the first output to disk
                if (first)
                {
                    first = false;
                    SaveBlobToFile(blobLocation, string.Format("The results for {0}", output.Key));
                }
            }
            return "done";
        }

        public static string InvokeBatchExecutionService()
        {
            // How this works:
            //
            // 1. Assume the input is present in a local file (if the web service accepts input)
            // 2. Upload the file to an Azure blob - you'd need an Azure storage account
            // 3. Call the Batch Execution Service to process the data in the blob. Any output is written to Azure blobs.
            // 4. Download the output blob, if any, to local file

            const string BaseUrl = "https://ussouthcentral.services.azureml.net/workspaces/b78cb51f8e1941598ccc6b3f1aa4ffb5/services/331dc6f2f8fc44bc89cf4d3195496c52/jobs";

            const string StorageAccountName = "adsfinalteam3"; // Replace this with your Azure Storage Account name
            const string StorageAccountKey = "p7nFH+iLX9emmXsVTwmSMGKvZ2b4QcITNNGSIPZCR1aT+5yaGZHZEYg+Kcy0Qud6DjDtjCQPKw9InEq+1zwO8A=="; // Replace this with your Azure Storage Key
            const string StorageContainerName = "adsfinaldata"; // Replace this with your Azure Storage Container name
            const string apiKey = "jUFXNd5oH4N0Vcw2FjKlNfSgZqPgz+vCDuCqNnYQpB8HjF9eDPNMK8rQdldVIFwu5F1PsrgoRdlLKHPUOqHKMQ=="; // Replace this with the API key for the web service

            // set a time out for polling status
            const int TimeOutInMilliseconds = 500 * 1000; // Set a timeout of 2 minutes


            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);
            String pth = System.Web.HttpContext.Current.Server.MapPath("~");
            pth += "/Content/";
            UploadFileToBlob(pth+ "SingleUser.csv" /*Replace this with the location of your input file*/,
               "SingleUserblob.csv" /*Replace this with the name you would like to use for your Azure blob; this needs to have the same extension as the input file */,
               StorageContainerName, storageConnectionString);

            using (HttpClient client = new HttpClient())
            {
                var request = new BatchExecutionRequest()
                {
                    Inputs = new Dictionary<string, AzureBlobDataReference>()
                    {

                        {
                            "input1",
                            new AzureBlobDataReference()
                            {
                                ConnectionString = storageConnectionString,
                                RelativeLocation = string.Format("{0}/SingleUserblob.csv.csv", StorageContainerName)
                            }
                        },
                    },

                    Outputs = new Dictionary<string, AzureBlobDataReference>()
                    {

                        {
                            "output1",
                            new AzureBlobDataReference()
                            {
                                ConnectionString = storageConnectionString,
                                RelativeLocation = string.Format("/{0}/output1results.csv", StorageContainerName)
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                Console.WriteLine("Submitting the job...");

                // submit the job
                var response = client.PostAsJsonAsync(BaseUrl + "?api-version=2.0", request).Result;
                if (!response.IsSuccessStatusCode)
                {
                    WriteFailedResponse(response);
                    return "failatsubmit";
                }

                string jobId =  response.Content.ReadAsAsync<string>().Result;
                Console.WriteLine(string.Format("Job ID: {0}", jobId));


                // start the job
                Console.WriteLine("Starting the job...");
                response =  client.PostAsync(BaseUrl + "/" + jobId + "/start?api-version=2.0", null).Result;
                if (!response.IsSuccessStatusCode)
                {
                    WriteFailedResponse(response);
                    return "failatstart";
                }

                string jobLocation = BaseUrl + "/" + jobId + "?api-version=2.0";
                Stopwatch watch = Stopwatch.StartNew();
                bool done = false;
                while (!done)
                {
                    Console.WriteLine("Checking the job status...");
                    response = client.GetAsync(jobLocation).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        WriteFailedResponse(response);
                        return "failatcheck";
                    }

                    BatchScoreStatus status = response.Content.ReadAsAsync<BatchScoreStatus>().Result;
                    if (watch.ElapsedMilliseconds > TimeOutInMilliseconds)
                    {
                        done = true;
                        Console.WriteLine(string.Format("Timed out. Deleting job {0} ...", jobId));
                        client.DeleteAsync(jobLocation);
                    }
                    switch (status.StatusCode)
                    {
                        case BatchScoreStatusCode.NotStarted:
                            Console.WriteLine(string.Format("Job {0} not yet started...", jobId));
                            break;
                        case BatchScoreStatusCode.Running:
                            Console.WriteLine(string.Format("Job {0} running...", jobId));
                            break;
                        case BatchScoreStatusCode.Failed:
                            Console.WriteLine(string.Format("Job {0} failed!", jobId));
                            Console.WriteLine(string.Format("Error details: {0}", status.Details));
                            done = true;
                            break;
                        case BatchScoreStatusCode.Cancelled:
                            Console.WriteLine(string.Format("Job {0} cancelled!", jobId));
                            done = true;
                            break;
                        case BatchScoreStatusCode.Finished:
                            done = true;
                            Console.WriteLine(string.Format("Job {0} finished!", jobId));

                            return ProcessResults(status);
                            
                    }

                    if (!done)
                    {
                        Thread.Sleep(1000); // Wait one second
                    }
                }
            }
            return "";   
        }
    }
}

