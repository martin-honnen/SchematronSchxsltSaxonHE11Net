using net.sf.saxon.s9api;
using net.liberty_development.SaxonHE11s9apiExtensions;
using System.Reflection;
using System.Diagnostics;
using net.sf.saxon.lib;

namespace SchematronSchxsltSaxonHE11Net
{
    internal class Program
    {
        static string schxsltSvrlXsltResource = "/xslt/2.0/pipeline-for-svrl.xsl";

        static void Main(string[] args)
        {
            Console.WriteLine($"Schematron Schxslt Validator using Schxslt 1.9.4 and Saxon HE 11.4 under {Environment.Version} {Environment.OSVersion}");

            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: SchematronSchxsltSaxonHE11Net schema.sch input.xml");
                return;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            // load Schxslt assembly
            var schxsltAssembly = Assembly.Load("schxslt");
            ikvm.runtime.Startup.addBootClassPathAssembly(schxsltAssembly);

            // force loading of updated xmlresolver
            ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver"));
            ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("org.xmlresolver.xmlresolver_data"));

            var processor = new Processor(false);

            var xsltCompiler = processor.newXsltCompiler();

            var jarResolver = new JarResolver();

            xsltCompiler.setResourceResolver(jarResolver);

            var compiledSchxslt = xsltCompiler.compile(jarResolver.resolve(new ResourceRequest() { baseUri = "schxslt", relativeUri = schxsltSvrlXsltResource })).load30();

            var compiledSchematron = new XdmDestination();
            compiledSchematron.setBaseURI(new Uri(new FileInfo(args[0]).FullName).ToURI());

            compiledSchxslt.Transform(new FileInfo(args[0]), compiledSchematron);

            var xsltCompiler2 = processor.newXsltCompiler();

            var schematronValidator = xsltCompiler2.compile(compiledSchematron.getXdmNode().asSource()).load30();

            var svrlResult = new XdmDestination();

            schematronValidator.Transform(new FileInfo(args[1]), svrlResult);

            var valid = processor.newXPathCompiler().evaluateSingle("not((/Q{http://purl.oclc.org/dsdl/svrl}schematron-output!(Q{http://purl.oclc.org/dsdl/svrl}failed-assert , Q{http://purl.oclc.org/dsdl/svrl}successful-report)))", svrlResult.getXdmNode()).getUnderlyingValue().effectiveBooleanValue();

            Console.WriteLine($"XML document {args[1]} is {(valid ? "" : "not ")}valid against Schematron schema {args[0]}.");

            if (!valid)
            {
                Console.WriteLine($"{Environment.NewLine}Validation report:{Environment.NewLine}{svrlResult.getXdmNode()}");
            }

            stopWatch.Stop();

            Console.WriteLine($"Elapsed time: {stopWatch.Elapsed}");
        }
    }
}