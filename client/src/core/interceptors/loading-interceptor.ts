import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cache = new Map<string, HttpEvent<unknown>>(); // we gonna cache based on the string and the string in this case is going to represent the URL the request is going to and we are going to cache the get request we are not interested in caching that is not a get request

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService)

  // if(req.method === 'GET'){
  //   const cachedResponse = cache.get(req.url);
  //   if(cachedResponse){
  //     return of(cachedResponse);
  //   }
  // }
  busyService.busy();

  return next(req).pipe(
    delay(500), 
    tap(response => {
      cache.set(req.url, response)
    }),
    finalize(() => {
      busyService.idel();
    })
  )
};
