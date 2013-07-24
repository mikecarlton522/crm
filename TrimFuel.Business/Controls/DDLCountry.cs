using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Model;

namespace TrimFuel.Business.Controls
{
    public class DDLCountry : DropDownList
    {
        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            IList<Country> countries = (new PageService()).GetCountries();
            foreach (var c in countries)
            {
                c.Code = c.Name;
            }
            countries.Add(new Country() { Name = "United States", Code = RegistrationService.DEFAULT_COUNTRY });
            countries = countries.OrderBy(i => i.Name).ToList();
            countries = countries.OrderBy(i => i.Code, new SortCountries()).ToList();

            foreach (Country country in countries)
            {
                Items.Add(new ListItem(country.Name, country.Code));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }

        private class SortCountries : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y)
                    return 0;

                if (x == RegistrationService.DEFAULT_COUNTRY)
                    return -1;
                else
                {
                    if (x == RegistrationService.UK_COUNTRY)
                    {
                        if (y == RegistrationService.DEFAULT_COUNTRY)
                            return 1;
                        else
                            return -1;
                    }
                }

                if (y == RegistrationService.DEFAULT_COUNTRY)
                    return 1;
                else
                {
                    if (y == RegistrationService.UK_COUNTRY)
                    {
                        if (x == RegistrationService.DEFAULT_COUNTRY)
                            return -1;
                        else
                            return 1;
                    }
                }

                return string.Compare(x, y);
            }
        }
    }
}