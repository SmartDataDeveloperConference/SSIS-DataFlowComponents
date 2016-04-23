using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Sddc.SampleDataFlowComponent
{
    /// <summary>
    /// Provides an user interface for the component.
    /// </summary>
    public class SddcSampleDataFlowComponentUI : IDtsComponentUI
    {
        /// <summary>
        /// The components metadata.
        /// </summary>
        private IDTSComponentMetaData100 componentMeta;

        /// <summary>
        /// Called when user deletes the component from SSIS package.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public void Delete(System.Windows.Forms.IWin32Window parentWindow)
        {
        }

        /// <summary>
        /// Called when user double clicks the component to edit it.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        /// <param name="variables">The variables.</param>
        /// <param name="connections">The connections.</param>
        /// <returns>Boolean indicating where component has changed.</returns>
        public bool Edit(System.Windows.Forms.IWin32Window parentWindow, Microsoft.SqlServer.Dts.Runtime.Variables variables, Microsoft.SqlServer.Dts.Runtime.Connections connections)
        {
            var dialog = new SetWebsiteAddressDialog();

            var websiteAddressProperty = componentMeta.CustomPropertyCollection["Website Address"];
            dialog.WebsiteAddress = websiteAddressProperty.Value;

            if (dialog.ShowDialog(parentWindow) == DialogResult.OK)
            {
                websiteAddressProperty.Value = dialog.WebsiteAddress;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when user requests help.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public void Help(System.Windows.Forms.IWin32Window parentWindow)
        {
        }

        /// <summary>
        /// Called on initialization.
        /// </summary>
        /// <param name="dtsComponentMetadata">The components metadata.</param>
        /// <param name="serviceProvider">A service provider.</param>
        public void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            componentMeta = dtsComponentMetadata;
        }

        /// <summary>
        /// Called when the user drags the component to the SSIS package.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        public void New(System.Windows.Forms.IWin32Window parentWindow)
        {
        }
    }
}
