using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HtmlTextHelper
{
    Dictionary<string, char> specialCharacters;
    const char specialCharStart = '&';
    const char specialCharEnd = ';';
    const char htmlCharStart = '<';
    const char htmlCharEnd = '>';
    const int maxSpecialLength = 50;

    public HtmlTextHelper()
    {
        specialCharacters = new Dictionary<string, char>();
               

        // Special Characters
        specialCharacters.Add("&Agrave;" ,'À'  );
        specialCharacters.Add("&Aacute;" ,'Á'  );
        specialCharacters.Add("&Acirc;"  ,'Â'  );
        specialCharacters.Add("&Atilde;" ,'Ã'  );
        specialCharacters.Add("&Auml;"   ,'Ä'  );
        specialCharacters.Add("&Aring;"  ,'Å'  );
        specialCharacters.Add("&agrave;" ,'à'  );
        specialCharacters.Add("&aacute;" ,'á'  );
        specialCharacters.Add("&acirc;"  ,'â'  );
        specialCharacters.Add("&atilde;" ,'ã'  );
        specialCharacters.Add("&auml;"   ,'ä'  );
        specialCharacters.Add("&aring;"  ,'å'  );
        specialCharacters.Add("&AElig;"  ,'Æ'  );
        specialCharacters.Add("&aelig;"  ,'æ'  );
        specialCharacters.Add("&szlig;"  ,'ß'  );
        specialCharacters.Add("&Ccedil;" ,'Ç'  );
        specialCharacters.Add("&ccedil;" ,'ç'  );
        specialCharacters.Add("&Egrave;" ,'È'  );
        specialCharacters.Add("&Eacute;" ,'É'  );
        specialCharacters.Add("&Ecirc;"  ,'Ê'  );
        specialCharacters.Add("&Euml;"   ,'Ë'  );
        specialCharacters.Add("&egrave;" ,'è'  );
        specialCharacters.Add("&eacute;" ,'é'  );
        specialCharacters.Add("&ecirc;"  ,'ê'  );
        specialCharacters.Add("&euml;"   ,'ë'  );
        specialCharacters.Add("&#131;"   ,'ƒ'  );
        specialCharacters.Add("&Igrave;" ,'Ì'  );
        specialCharacters.Add("&Iacute;" ,'Í'  );
        specialCharacters.Add("&Icirc;"  ,'Î'  );
        specialCharacters.Add("&Iuml;"   ,'Ï'  );
        specialCharacters.Add("&igrave;" ,'ì'  );
        specialCharacters.Add("&iacute;" ,'í'  );
        specialCharacters.Add("&icirc;"  ,'î'  );
        specialCharacters.Add("&iuml;"   ,'ï'  );
        specialCharacters.Add("&Ntilde;" ,'Ñ'  );
        specialCharacters.Add("&ntilde;" ,'ñ'  );
        specialCharacters.Add("&Ograve;" ,'Ò'  );
        specialCharacters.Add("&Oacute;" ,'Ó'  );
        specialCharacters.Add("&Ocirc;"  ,'Ô'  );
        specialCharacters.Add("&Otilde;" ,'Õ'  );
        specialCharacters.Add("&Ouml;"   ,'Ö'  );
        specialCharacters.Add("&ograve;" ,'ò'  );
        specialCharacters.Add("&oacute;" ,'ó'  );
        specialCharacters.Add("&ocirc;"  ,'ô'  );
        specialCharacters.Add("&otilde;" ,'õ'  );
        specialCharacters.Add("&ouml;"   ,'ö'  );
        specialCharacters.Add("&Oslash;" ,'Ø'  );
        specialCharacters.Add("&oslash;" ,'ø'  );
        specialCharacters.Add("&#140;"   ,'Œ'  );
        specialCharacters.Add("&#156;"   ,'œ'  );
        specialCharacters.Add("&#138;"   ,'Š'  );
        specialCharacters.Add("&#154;"   ,'š'  );
        specialCharacters.Add("&Ugrave;" ,'Ù'  );
        specialCharacters.Add("&Uacute;" ,'Ú'  );
        specialCharacters.Add("&Ucirc;"  ,'Û'  );
        specialCharacters.Add("&Uuml;"   ,'Ü'  );
        specialCharacters.Add("&ugrave;" ,'ù'  );
        specialCharacters.Add("&uacute;" ,'ú'  );
        specialCharacters.Add("&ucirc;"  ,'û'  );
        specialCharacters.Add("&uuml;"   ,'ü'  );
        specialCharacters.Add("&#181;"   ,'µ'  );
        specialCharacters.Add("&#215;"   ,'×'  );
        specialCharacters.Add("&Yacute;" ,'Ý'  );
        specialCharacters.Add("&#159;"   ,'Ÿ'  );
        specialCharacters.Add("&yacute;" ,'ý'  );
        specialCharacters.Add("&yuml;"   ,'ÿ'  );
        specialCharacters.Add("&#176;"   ,'°'  );
        specialCharacters.Add("&#134;"   ,'†'  );
        specialCharacters.Add("&#135;"   ,'‡'  );
        specialCharacters.Add("&lt;"     ,'<'  );
        specialCharacters.Add("&gt;"     ,'>'  );
        specialCharacters.Add("&#177;"   ,'±'  );
        specialCharacters.Add("&#171;"   ,'«'  );
        specialCharacters.Add("&#187;"   ,'»'  );
        specialCharacters.Add("&#191;"   ,'¿'  );
        specialCharacters.Add("&#161;"   ,'¡'  );
        specialCharacters.Add("&#183;"   ,'·'  );
        specialCharacters.Add("&#149;"   ,'•'  );
        specialCharacters.Add("&#153;"   ,'™'  );
        specialCharacters.Add("&copy;"   ,'©'  );
        specialCharacters.Add("&reg;"    ,'®'  );
        specialCharacters.Add("&#167;"   ,'§'  );
        specialCharacters.Add("&#182;"   ,'¶'  );
        
    }


    public string MakeUnityFriendly(string htmlText, out Dictionary<string,string> hyperlinks)
    {
        hyperlinks = new Dictionary<string, string>();
        char[] htmlChars = htmlText.ToCharArray();
        int numberOfHtmlChars = htmlChars.Length;

        StringBuilder convertedTextBuilder = new StringBuilder(numberOfHtmlChars, numberOfHtmlChars + 100);
        StringBuilder specialCharBuilder = new StringBuilder(maxSpecialLength, maxSpecialLength);

        bool isBuildingSpecialCharCode = false;
        bool isBuildingHtmlCode = false;
        int specialCharCount = 0;

        for(int i = 0, ni = htmlChars.Length; i<ni; i++)
        {
            char currentChar = htmlChars[i];

            //Are these the start of a special character?
            if (currentChar == htmlCharStart)
            {
                //Started a new possible html char, clear the buffer
                convertedTextBuilder.Append(specialCharBuilder);
                specialCharBuilder.Clear();

                specialCharCount = 0;
                isBuildingHtmlCode = true;
                isBuildingSpecialCharCode = false;
            }
            else if (currentChar == specialCharStart)
            {
                //Started a new possible special char, clear the buffer
                convertedTextBuilder.Append(specialCharBuilder);
                specialCharBuilder.Clear();

                specialCharCount = 0;
                isBuildingHtmlCode = false;
                isBuildingSpecialCharCode = true;
            }


            if(isBuildingSpecialCharCode || isBuildingHtmlCode)
            {
                //If we are building a possible special character
                specialCharBuilder.Append(currentChar);
                specialCharCount++;

                //Check if we have reached the end
                if(isBuildingHtmlCode && currentChar == htmlCharEnd)
                {
                    string htmlCharCode = specialCharBuilder.ToString();

                    //TODO Handle any possible HTML code
                    switch(htmlCharCode)
                    {
                        case "<p>":
                            //Start of a paragraph. Do nothing
                            break;
                        case "</p>":
                            //End of a paragraph
                            convertedTextBuilder.Append("\n\n");
                            break;
                        case "<strong>":
                            //Strong text. Replace with bold
                            convertedTextBuilder.Append("<b>");
                            break;
                        case "</strong>": 
                            //Strong text. Replace with bold
                            convertedTextBuilder.Append("</b>");
                            break;
                        case "<b>":                           
                        case "</b>":
                        case "<i>":
                        case "</i>":
                            //If bold or italic start/end chars
                            convertedTextBuilder.Append(htmlCharCode);
                            break;
                        case "</a>":
                            //End of a hyperlink
                            convertedTextBuilder.Append("</link>");
                            convertedTextBuilder.Append("</color>");
                            break;
                        default:
                            Debug.Log("Unknown HTML: " + htmlCharCode);

                            //If we reached here, this is an unusual instance and could be a HTML hyperlink
                            if (htmlCharCode.Contains("href"))
                            {
                                string[] hyperlinkSplit = htmlCharCode.Split('"');

                                if (hyperlinkSplit.Length == 3)
                                {
                                    // A valid hyperlink would contain 2 " characters, giving us 3 parts.

                                    string id = "html" + (hyperlinks.Count);
                                    string link = hyperlinkSplit[1]; //The middle is the link

                                    hyperlinks.Add(id, link);
                                    Debug.Log(link);

                                    convertedTextBuilder.Append(string.Format("<color=#FACA26><link=\"{0}\">", id));
                                }
                                else if (hyperlinkSplit.Length == 5)
                                {
                                    // e.g. <a href="" target="">Privacy Policy</a> We ignore the target
                                    string id = "html" + (hyperlinks.Count);
                                    string link = hyperlinkSplit[1]; //The middle is the link

                                    hyperlinks.Add(id, link);
                                    Debug.Log(link);

                                    convertedTextBuilder.Append(string.Format("<color=#FACA26><link=\"{0}\">", id));

                                }
                            }
                            break;
                    }

                    specialCharBuilder.Clear();

                    specialCharCount = 0;
                    isBuildingSpecialCharCode = false;
                }
                else if(isBuildingSpecialCharCode && currentChar == specialCharEnd)
                {
                    string specialCharCode = specialCharBuilder.ToString();

                    if(specialCharCode == "&nbsp;")
                    {
                        //This is a non breaking space character. It will insert a space, but the words on either side cannot be on new lines

                        //TODO using <nobr>I M P O R T A N T</nobr>

                        convertedTextBuilder.Append(' ');
                    }
                    else if(specialCharacters.ContainsKey(specialCharCode))
                    {
                        convertedTextBuilder.Append(specialCharacters[specialCharCode]);
                    }
                    else
                    {
                        Debug.LogError("HTML: Unknown character code: " + specialCharCode);
                    }

                    specialCharBuilder.Clear();

                    specialCharCount = 0;
                    isBuildingSpecialCharCode = false;
                }
                else if(specialCharCount >=maxSpecialLength)
                {
                    //if we reach the limit of a special character
                    //clear out what we stole and give it to the main builder
                    convertedTextBuilder.Append(specialCharBuilder);
                    specialCharBuilder.Clear();

                    specialCharCount = 0;
                    isBuildingHtmlCode = false;
                    isBuildingSpecialCharCode = false;
                }
            }
            else
            {
                //If not building a special character, append it to the main builder
                convertedTextBuilder.Append(currentChar);
            }
        }

        //If we have any left overs, append them
        if(specialCharCount > 0)
        {
            convertedTextBuilder.Append(specialCharBuilder);
            specialCharBuilder.Clear();
        }


        return convertedTextBuilder.ToString();
    }
}
