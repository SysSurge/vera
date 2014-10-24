using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

namespace VeraWAF.WebPages.Bll.VirtualPathProvider
{
    public class CustomVirtualDirectory : VirtualDirectory {
        readonly CustomVirtualPathProvider _spp;

        public bool Exists { get; private set; }

        public CustomVirtualDirectory(string virtualDir, CustomVirtualPathProvider provider) : base(virtualDir) {
            _spp = provider;
            GetData();
        }

        protected void GetData() {
            var virtualFiles = _spp.GetVirtualData();

            if (virtualFiles.Any(virtualFile => virtualFile.VirtualPath.StartsWith(VirtualPath)))
            {
                Exists = true;

                var prosessed = new Dictionary<string, string>();

                foreach (var virtualFile in virtualFiles.Where(vf => vf.VirtualPath.StartsWith(VirtualPath)))
                {
                    var childPath = virtualFile.VirtualPath.Substring(VirtualPath.Length);
                    var subElements = childPath.Split('/');
                    var subElement = VirtualPath + subElements[0];

                    if (!prosessed.ContainsKey(subElement))
                    {
                        prosessed.Add(subElement, String.Empty);

                        switch (subElements.Count())
                        {
                            case 1:
                                // Is a file
                                var svf = new CustomVirtualFile(subElement, _spp);
                                children.Add(svf);
                                files.Add(svf);
                                break;
                            case 2:
                                // Is a child directory
                                var svd = new CustomVirtualDirectory(subElement, _spp);
                                children.Add(svd);
                                directories.Add(svd);
                                break;
                        }
                    }
                }

            }
        }

        private ArrayList children = new ArrayList();
        public override IEnumerable Children {
            get { return children; }
        }

        private ArrayList directories = new ArrayList();
        public override IEnumerable Directories {
            get { return directories; }
        }

        private ArrayList files = new ArrayList();
        public override IEnumerable Files {
            get { return files; }
        }
    }
}