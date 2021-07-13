import sys
instring = "test"
string = f"{sys.argv[1]}"
print(string)



# import glob, os
# import re
# for file in glob.glob("./**/**/*.md", recursive= True):
#     with open(file, encoding="utf8") as markdownFile:
#         markdown = markdownFile.read()
#         if "Estimated reading time:" in markdown:
#             print(markdownFile.name)
#         if "(#h-" in markdown:
#             print(markdownFile.name)
#         #image = file.replace("/index.md", "")
#         #githubPath = "https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/" + image
#         # markdown = re.sub(r'images/', githubPath + '/images/', markdown)
#         markdown = re.sub(r'(?s)^---.+---\n', '', markdown)
#         strings = re.findall(r'[#\n]+# .+', markdown)

#         markdownFile.close()
#         writeFile = open(file, encoding="utf8", mode="w")
#         writeFile.write(markdown)
#         writeFile.close()

#     id = string.lower()
#     # id = id.replace(".net", "net")
#     id = id.replace(" ", "-")
#     id = id.replace("...", "-")
#     id = id.replace("?", "")
#     id = id.replace("\n", "")
#     id = id.replace("#", "")
#     id = id.replace("'", "-")
#     id = id.replace("{}", "-")
#     id = id.replace("[]", "-")
#     id = id.replace(":", "")
#     id = id.replace(".", "")
#     id = id.replace(",", "")
#     id = id.replace("!", "")
#     id = id.replace("(", "")
#     id = id.replace(")", "")
#     id = id.replace("[", "")
#     id = id.replace("]", "")
#     id = id.replace("<", "")
#     id = id.replace(">", "")
#     id = id.replace("&", "")
#     id = id.replace("---", "-")
#     id = id.replace("--", "-")