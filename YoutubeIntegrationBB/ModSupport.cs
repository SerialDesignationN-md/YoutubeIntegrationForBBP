using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace YoutubeIntegrationBB
{
    internal class ModSupport
    {
        private void addModSupportCommand(string id, string text, string type, [Optional] object[] args)
        {
            var CPH = BasePlugin.Instance.CPH;
            CPH.NewCommand(id, text, type, args);
        }

        public void AddModSupport()
        {
            
        }
    }
}
