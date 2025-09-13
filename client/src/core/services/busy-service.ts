import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = signal(0);

  busy(){
    this.busyRequestCount.update(current => current + 1);
  }

  idel(){
    this.busyRequestCount.update(current => Math.max(current - 1));
  }

}
