using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FunkyCustomerCare.Integration.Tests.Util
{
    public class AssemblyResourceFileReader
    {
        public async Task<string> GetFileContentAsync(string fileName, string extension="json")
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }
            try
            {
                extension = string.IsNullOrWhiteSpace(extension) ? "json" : extension;
                fileName = Path.ChangeExtension(fileName, extension);

                var assembly = typeof(AssemblyResourceFileReader).Assembly;

                using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.MockData.{fileName}"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var content = await reader.ReadToEndAsync();
                        return content;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return string.Empty;
        }
    }
}
