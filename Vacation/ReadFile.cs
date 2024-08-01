using System;
using Google.Protobuf.WellKnownTypes;

namespace Vacation
{
	public class ReadFile
	{
		string filePath = "Vault.txt";
        string value = "";

		public string ReadFileCreds(string apiType) {
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(apiType))
                            {
                                //split into array of 2 by =
                                string[] parts = line.Split(':');

                                //trim front and back white spaces
                                value = parts[1].Trim();
                                return value;
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("issue reading file");
                }
            }
            else
            {
                Console.WriteLine("File does not exist");
            }
            return "";
        }

	}
}

