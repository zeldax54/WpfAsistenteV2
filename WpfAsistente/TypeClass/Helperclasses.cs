using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAsistente.TypeClass
{




    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Button
    {

        private string nameField;

        private string imageNameField;

        private string actionField;

        private string videonameField;

        private string browserURLField;

        private bool onlyVideoField;

        private bool onlyBrowserField;

        private bool fromRightField;

        private decimal postitionField;

        private decimal fromBottonField;

        private decimal sizeField;

        private decimal startAnimationTimeField;

        private ButtonDelchild[] deletesectionsField;

        private string[] allowedurlscopeField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string ImageName
        {
            get
            {
                return this.imageNameField;
            }
            set
            {
                this.imageNameField = value;
            }
        }

        /// <remarks/>
        public string Action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        public string videoname
        {
            get
            {
                return this.videonameField;
            }
            set
            {
                this.videonameField = value;
            }
        }

        /// <remarks/>
        public string BrowserURL
        {
            get
            {
                return this.browserURLField;
            }
            set
            {
                this.browserURLField = value;
            }
        }

        /// <remarks/>
        public bool OnlyVideo
        {
            get
            {
                return this.onlyVideoField;
            }
            set
            {
                this.onlyVideoField = value;
            }
        }

        /// <remarks/>
        public bool OnlyBrowser
        {
            get
            {
                return this.onlyBrowserField;
            }
            set
            {
                this.onlyBrowserField = value;
            }
        }

        /// <remarks/>
        public bool FromRight
        {
            get
            {
                return this.fromRightField;
            }
            set
            {
                this.fromRightField = value;
            }
        }

        /// <remarks/>
        public decimal Postition
        {
            get
            {
                return this.postitionField;
            }
            set
            {
                this.postitionField = value;
            }
        }

        /// <remarks/>
        public decimal FromBotton
        {
            get
            {
                return this.fromBottonField;
            }
            set
            {
                this.fromBottonField = value;
            }
        }

        /// <remarks/>
        public decimal Size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        public decimal startAnimationTime
        {
            get
            {
                return this.startAnimationTimeField;
            }
            set
            {
                this.startAnimationTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("delchild", IsNullable = false)]
        public ButtonDelchild[] deletesections
        {
            get
            {
                return this.deletesectionsField;
            }
            set
            {
                this.deletesectionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("url", IsNullable = false)]
        public string[] allowedurlscope
        {
            get
            {
                return this.allowedurlscopeField;
            }
            set
            {
                this.allowedurlscopeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ButtonDelchild
    {

        private string tagField;

        private string typeField;

        private string nameField;

        /// <remarks/>
        public string tag
        {
            get
            {
                return this.tagField;
            }
            set
            {
                this.tagField = value;
            }
        }

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }










}
