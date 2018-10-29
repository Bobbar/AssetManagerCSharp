using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace AssetManager.Tools.Deployment
{
    /// <summary>
    /// Provides simple XML parsing for deployment command string and settings.
    /// </summary>
    /// <example>
    /// 
    ///  var test = new XmlSettings(@"..\DeploymentSettings.xml");
    ///  var result = test.GetDeploymentString("mvps_install");
    ///  Console.WriteLine(result);
    ///  
    /// </example>
    public sealed class XmlSettings
    {
        private XElement _deploymentElem;

        public XmlSettings(string settingsFileUri)
        {
            _deploymentElem = XElement.Load(settingsFileUri, LoadOptions.None);
        }

        public XmlSettings(Stream settingsFileStream)
        {
            _deploymentElem = XElement.Load(settingsFileStream, LoadOptions.None);
        }

        public string GetDeploymentString(string name)
        {
           try
            {
                // Get all the settings elements in the XML.
                var elements = _deploymentElem.DescendantsAndSelf();

                // Get the deployment strings elements.
                var strings = elements.Descendants().Where(e => e.Name.LocalName == "DeploymentStrings");

                // Get all the command elements.
                var commands = strings.Elements().Where(e => e.Name.LocalName == "Command");

                // Get the "Name" attribute.
                var attrib = commands.Attributes().Where(a => a.Name.LocalName == "Name").First().Name;

                // Get the command element where the Name attribute matches the specified value.
                var requestedElement = commands.Where(c => c.Attribute(attrib).Value == name).First();

                // Return the value of the found command element.
                return requestedElement.Value.Trim();
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }
        }
    }
}
