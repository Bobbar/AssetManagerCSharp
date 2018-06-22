using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.CustomControls
{
    public interface IWindowList
    {

        event EventHandler<ExtendedForm> ChildAdded;

        event EventHandler<ExtendedForm> ChildRemoved;

        /// <summary>
        /// List of child forms bound to the current form. This list ultimately comprises a tree of parent and child forms.
        /// </summary>
        List<ExtendedForm> ChildForms { get; }

        /// <summary>
        /// The total number of child forms within the tree.
        /// </summary>
        /// <returns></returns>
        int ChildFormCount();
    }
}
