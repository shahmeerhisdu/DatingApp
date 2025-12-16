import { Component, computed, input, model, output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  imports: [],
  templateUrl: './paginator.html',
  styleUrl: './paginator.css'
})
export class Paginator {
  // we want the page number to be an input from the component above, so we have looked in the input properties previously and they allow us to recieve something from the parent component, but input properties are not writebale signals, so we need to recieve something because we will be using Paginator in the paginator template, we also need it to be writeable so that when the user clicks on a different page number we can update the page number. So we have the special signal that is called model singal that allows us to do that. Its both an input and writable.
  pageNumber = model(1);
  pageSize = model(10);
  totalCount = input(0);
  pageSizeOptions = input([5, 10, 20, 50]);

  // we will have the output property with the name pageChange that we will send to the parent component when the user clicks on a different page number, because we need to send this information to our service so that it can go and request the next batch of pages.

  pageChange = output<{ pageNumber: number, pageSize: number }>();


  // now inside our template I also want to display a message that how many items out of the total items are being shown on the current page, so for that we need to know the last idex of the batch of items we have rceived from the server.

  lastItemIndex = computed(() => {
    return Math.min(this.pageNumber() * this.pageSize(), this.totalCount());
  })

  onPageChange(newPage?: number, pageSize?: EventTarget | null) {
    if (newPage) {
      this.pageNumber.set(newPage);
    }
    if (pageSize) {
      const size = Number((pageSize as HTMLSelectElement).value);
      this.pageSize.set(size);
    }

    this.pageChange.emit({ pageNumber: this.pageNumber(), pageSize: this.pageSize() });

  }
}
