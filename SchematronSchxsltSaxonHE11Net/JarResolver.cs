using javax.xml.transform;
using javax.xml.transform.stream;
using net.sf.saxon.lib;
using JURI = java.net.URI;
using JURL = java.net.URL;

namespace SchematronSchxsltSaxonHE11Net
{
    internal class JarResolver : ResourceResolver
    {
        private readonly org.xmlresolver.Resolver resolver = new org.xmlresolver.Resolver();

        public Source resolve(ResourceRequest request)
        {
            string relativeUri = request.relativeUri;
            string baseUri = request.baseUri;

            JURI baseURI;
            JURI hrefURI;

            if (baseUri == null || baseUri == String.Empty)
            {
                baseURI = new JURI("");
            }
            else
            {
                baseURI = new JURI(baseUri.Substring(1 + baseUri.IndexOf("!")));
            }

            if (relativeUri == String.Empty)
            {
                hrefURI = baseURI;
            }
            else
            {
                hrefURI = baseURI.resolve(relativeUri);
            }

            java.lang.Class clazz = typeof(JarResolver);
            JURL systemId = clazz.getResource(hrefURI.toString());

            if (systemId != null)
            {
                Source source = new StreamSource(clazz.getResourceAsStream(hrefURI.toString()));
                source.setSystemId(systemId.toString());
                return source;
            }

            return resolver.resolve(relativeUri, baseUri);
        }
    }
}
