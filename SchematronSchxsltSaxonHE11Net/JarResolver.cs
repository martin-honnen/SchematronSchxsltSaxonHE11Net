using javax.xml.transform;
using javax.xml.transform.stream;
using JURI = java.net.URI;
using JURL = java.net.URL;

namespace SchematronSchxsltSaxonHE11Net
{
    internal class JarResolver : URIResolver
    {
        private readonly org.xmlresolver.Resolver resolver = new org.xmlresolver.Resolver();
        public Source resolve(string href, string @base)
        {
            JURI baseUri;
            JURI hrefUri;

            if (@base == null || @base == String.Empty)
            {
                baseUri = new JURI("");
            }
            else
            {
                baseUri = new JURI(@base.Substring(1 + @base.IndexOf("!")));
            }

            if (href == String.Empty)
            {
                hrefUri = baseUri;
            }
            else
            {
                hrefUri = baseUri.resolve(href);
            }

            java.lang.Class clazz = typeof(JarResolver);
            JURL systemId = clazz.getResource(hrefUri.toString());

            if (systemId != null)
            {
                Source source = new StreamSource(clazz.getResourceAsStream(hrefUri.toString()));
                source.setSystemId(systemId.toString());

                return source;
            }

            return resolver.resolve(href, @base);
        }
    }
}
