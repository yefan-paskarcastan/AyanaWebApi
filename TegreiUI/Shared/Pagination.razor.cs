using Microsoft.AspNetCore.Components;

namespace TegreiUI.Shared
{
    public partial class Pagination
    {
        [Inject] public NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Максимальное число страниц
        /// </summary>
        [Parameter] public int MaxPage { get; set; }

        /// <summary>
        /// Событие, которое отрабатывает при смене страницы
        /// </summary>
        [Parameter] public EventCallback<int> ChangePageHandler { get; set; }

        /// <summary>
        /// Текущий номер страницы
        /// </summary>
        [Parameter] public int CurrentPage { get; set; }

        /// <summary>
        /// Адрес, куда будет переходить после клика
        /// </summary>
        [Parameter] public string Url { get; set; }

        async void ChangePage(int pageNumber)
        {
            NavigationManager.NavigateTo(Url + pageNumber);
            await ChangePageHandler.InvokeAsync(pageNumber);
        }
    }
}
