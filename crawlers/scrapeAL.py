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

def scrapeArticle(driver):
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    article_text = ""
    
    #Title
    if soup.find("h1", class_="page-title"):
        article_text = soup.find("h1", class_="page-title").text
        article_text += ". "
    else:
        return article_text

    #driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")

    #Subtitle
    #article_text += soup.find("", class_="").text

    #Body
    article = soup.find("div", class_="col-sm-8 col-sm-push-4 col-content")
    for p in article.find_all("p"):
        article_text += p.text  
    
    return article_text

def getArticles(driver, page_num, articles):
    print(driver.current_url)

    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    skipped_articles = 0

    #Getting the links to articles
    for tag in soup.find_all("div", class_="col-sm-4 col-xs-12 blog-entry clearfix"):
        link = tag.find("a", attrs={"class": None})
        articles += 1
        driver.get(link["href"])

        #Saving the articles
        if (articles < 10):
            with open("articles\\0{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\0{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping: file length {}".format(file_length))
                    articles -= 1
                    skipped_articles += 1

        else:
            with open("articles\\{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping: file length {}".format(file_length))
                    articles -= 1
                    skipped_articles += 1
        
        f.close

        print("{}: ".format(articles) + link["href"])

        if articles == 100:
            break

    if (articles < 100): 
        page_num += 1
        #Next page
        driver.get("https://alternativet.dk/nyheder/seneste?ccm_paging_p_b23485={}&ccm_order_by_b23485=cv.cvDatePublic&ccm_order_by_direction_b23485=desc".format(page_num))

        getArticles(driver, page_num, articles)

#Initial page with articles
driver.get("https://alternativet.dk/nyheder/seneste")

#Cookie notice
#WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, ""))).click()

getArticles(driver, page_num, articles)