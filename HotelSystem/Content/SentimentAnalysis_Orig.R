###################################################
# Sentiment Analysis - Final Project
# Advances in Data Sciences and Architecture
# Group 3
# Amitha, Divyansh, Jyoti
###################################################

#install.packages("wordcloud")
#install.packages("tm")
#install.packages("stringr")
#install.packages("plyr")
#install.packages("rjson")

library(wordcloud)
library(tm)
library(stringr)
library(plyr)
library(rjson)

function getscore(input){

  
  json_file <- paste("http://terminal2.expedia.com/x/reviews/hotels?hotelId=",input,"&apikey=JvMtGQVznEaKqvkEpYFZHSYEqINUhwhH",sep="",collapse ="")
  #Read data from file
  json_data <- fromJSON(file=json_file)
  
  
  reviewVector <- vector()
  #iterate through the reviews of hotel and read the text
  reviewOjectLength <- length(json_data$reviewDetails$reviewCollection$review)
  for(reviewNumber in 1:reviewOjectLength)
  {
    if(!(length(json_data$reviewDetails$reviewCollection$review) == 0))
    {
     # print("step1 : no review")
    if(!(is.null(json_data$reviewDetails$reviewCollection$review[[reviewNumber]])))
    {
    reviewVector <- c(reviewVector,json_data$reviewDetails$reviewCollection$review[[reviewNumber]]["reviewText"])
    }
    }
  }
  
  if(!(length(reviewVector) == 0))
  {
 # print("step2")
  #Converting vectors to matrix
 hotelReviewMatrix <- matrix(reviewVector ,ncol = 1, byrow=TRUE)
 
 # Create corpus
 corpus=Corpus(VectorSource(reviewVector))
 
 # Convert to lower-case
 corpus=tm_map(corpus,tolower)
 
 # Remove stopwords
 corpus=tm_map(corpus,function(x) removeWords(x,stopwords()))
 
 # convert corpus to a Plain Text Document
 corpus=tm_map(corpus,PlainTextDocument)
 
 col=brewer.pal(6,"Dark2")
 wordcloud(corpus, min.freq=25, scale=c(5,2),rot.per = 0.25,
           random.color=T, max.word=45, random.order=F,colors=col)
 
# print("step3")
 #lexicon based sentiment analysis
 positives= readLines("positive-words.txt")
 negatives = readLines("negative-words.txt")
 
 reviewScore <- data.frame()
 for(row in 1:nrow(hotelReviewMatrix))
 {
   review = hotelReviewMatrix[row,1]
 
   review = gsub("[[:punct:]]", "", review)    # remove punctuation
   review = gsub("[[:cntrl:]]", "", review)   # remove control characters
   #review = gsub('\d+', "", review)          # remove digits
   review = str_replace_all(review, "[^[:alnum:]]", " ")
   
   # Let's have error handling function when trying tolower
   tryTolower = function(x){
     # create missing value
     y = NA
     # tryCatch error
     try_error = tryCatch(tolower(x), error=function(e) e)
     # if not an error
     if (!inherits(try_error, "error"))
       y = tolower(x)
     # result
     return(y)
   }
   
   # use tryTolower with sapply
   review = sapply(review, tryTolower)
   
   require(stringr)
   # split sentence into words with str_split function from stringr package
   word_list = str_split(review, " ")
   words = unlist(word_list)
   
   # compare words to the dictionaries of positive & negative terms
   positive.matches = match(words, positives)
   negative.matches = match(words, negatives)
   
   # get the position of the matched term or NA
   # we just want a TRUE/FALSE
   positive_matches = !is.na(positive.matches)
   negative_matches = !is.na(negative.matches)
   
   # final score
   score = sum(positive_matches) - sum(negative_matches)
   reviewScore = c(reviewScore,score)
                  
 }         
 
# print("step4")
 
  overallHotelReviewScore <- sum(as.numeric(reviewScore),na.rm=FALSE)
  return(overallHotelReviewScore)
  }

  