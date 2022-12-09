import os
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.firefox.service import Service

abs_path = os.path.dirname(__file__)
rel_path = r"lib\\geckodriver.exe"

option = webdriver.FirefoxOptions()
option.headless = True
option.binary_location = r"C:\\Program Files\\Mozilla Firefox\\firefox.exe"
driverService = Service(os.path.join(abs_path, rel_path))
driver = webdriver.Firefox(service=driverService, options=option)

articles = 0
page_num = 1  

def getBody(driver, article_html, text):
    body = article_html.find("div", "et_pb_text_inner")
    for p in body.find_all("p"):
        text += p.text + " "
    return text

def getSubTitle(driver, article_html, text):
    subtitle = article_html.find("div", class_="et_pb_module et_pb_text et_pb_text_0 et_pb_text_align_left et_pb_bg_layout_light")
    text += subtitle.find("h4", attrs={"class": None}).text + " "
    body = article_html.find("div", class_="et_pb_module et_pb_text et_pb_text_1 et_pb_text_align_left et_pb_bg_layout_light")
    return getBody(driver, body, text)

def scrapeArticle(driver):
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    article_text = ""
    
    #Title
    article_text = soup.find("h1", class_="entry-title").text
    article_text += ". "

    #Finding the div containing the article in the parse tree
    article = soup.find("div", class_="et_pb_column et_pb_column_3_4 et_pb_column_1 et_pb_css_mix_blend_mode_passthrough")

    #If the article has a subtitle
    if article.find("div", class_="et_pb_module et_pb_text et_pb_text_1 et_pb_text_align_left et_pb_bg_layout_light") and article.find("div", class_="et_pb_module et_pb_text et_pb_text_0 et_pb_text_align_left et_pb_bg_layout_light"):
        #subheader
        article_text = getSubTitle(driver, article, article_text)
    else:
        article_text = getBody(driver, article, article_text)
        
    return article_text

def getArticles(driver, page_num, articles):
    print(driver.current_url)

    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    skipped_articles = 0
    article_index = articles

    #Getting the links to articles
    for tag in soup.find_all("a", class_="entry-featured-image-url"):
        articles += 1
        driver.get(tag["href"])

        #Saving the articles
        if (articles < 10):
            with open("articles\\0{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\0{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping")
                    articles -= 1
                    skipped_articles += 1

        else:
            with open("articles\\{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping")
                    articles -= 1
                    skipped_articles += 1
        
        f.close

        print("{}: ".format(articles) + tag["href"])

        if articles == 100:
            break

    if (article_index == articles):
        print("No new articles found!")
    elif (articles < 100): 
        page_num += 1
        #Next page
        driver.get("https://kd.dk/nyheder/page/{}".format(page_num))

        getArticles(driver, page_num, articles)

#Initial page with articles
driver.get("https://kd.dk/nyheder/")

getArticles(driver, page_num, articles)