using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using Newtonsoft.Json;
using AyanaWebApi.DTO;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace TegreiUI.Pages
{
    public partial class ProgramsList
    {
        [Parameter] public int _currentPage { get; set; }

        [Inject] public HttpClient HttpClient { get; set; }

        private IList<ListItem> _programsList;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (_currentPage == 0)
                _currentPage = 1;
            GetList(_currentPage);
        }

        async void GetList(int currentPage)
        {
            var pagination = new Pagination
            {
                CountItem = 50,
                CurrentPage = currentPage
            };
            string json = JsonConvert.SerializeObject(pagination);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await HttpClient.PostAsync("https://localhost:44334/UIContent/GetPrograms", content);
            if (result.IsSuccessStatusCode)
            {
                var contents = await result.Content.ReadAsStringAsync();
                _programsList = JsonConvert.DeserializeObject<IList<ListItem>>(contents);
                this.StateHasChanged();
            }
            else
            {
                _programsList = new List<ListItem>();
            }
        }
    }
}
