import json
import xml.etree.ElementTree as ET
import string
import codecs

def XML_name(json_name):
    return string.capwords(json_name)

def XML_element_for(json_element):
    element = ET.Element('Spell')
    for key, value in json_element.items():
        subElement = ET.Element(XML_name(key))
        subElement.text = value
        element.append(subElement)
    return element

def main():
    filename = "SpellList5e.json"
    with codecs.open(filename, 'r') as json_file:
        data = json.load(json_file)
    root = ET.Element("SpellDetails")
    for item in data:
        root.append(XML_element_for(item))
    with codecs.open("SpellDetails.xml", 'w', encoding='utf-8') as ofile:
        text = ET.tostring(root, encoding="unicode")
        ofile.write(text)
    return

if __name__ == "__main__":
    main()
