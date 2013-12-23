using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pikabu
{
    public interface IItemPage
    {
        void AppBarGoButtonEnablingDefinition(int id);
        void GoBack();
        void GoForward();
    }
}
