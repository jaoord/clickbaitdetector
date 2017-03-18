# What is this? #

* have a list of URL's
* for each url, detect all text that is inside <a></a> tags
* for each text inside the <a></a> tags, determine whether it is clickbait or genuine (need a good term)

of course the system needs to be trained with clickbait / genuine beforehand. 

## Todo ##
First, create a system that can do bayes categorization (https://github.com/joelmartinez/nBayes or http://accord-framework.net/ or http://www.shogun-toolbox.org/), 
After that, create a web scraper to retrieve content from <a> tags
Then determine the clickbait/genuine score
