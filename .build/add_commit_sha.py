import sys
def alter(file,old_str,new_str):
    file_data = ""
    with open(file, "r", encoding="utf-8") as f:
        for line in f:
            if old_str in line:
                line = line.replace(old_str,new_str)
            file_data += line
    with open(file,"w",encoding="utf-8") as f:
        f.write(file_data)
commit_sha = sys.argv[1]

alter("WonderLab/Views/Page/Setting/AboutPage.axaml", "<HyperlinkButton Content=\"2.0.0\"", "<HyperlinkButton Content=\"2.0.0 "+commit_sha[:7]+" build\"")
alter("WonderLab/Views/Page/Setting/AboutPage.axaml", "CommandParameter=\"https://github.com/Blessing-Studio/WonderLab.Override/\"", "CommandParameter=\"https://github.com/Blessing-Studio/WonderLab.Override/commit/"+commit_sha+"\"")

