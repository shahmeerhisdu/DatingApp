import { HttpEvent, HttpInterceptorFn, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cache = new Map<string, HttpEvent<unknown>>(); // we gonna cache based on the string and the string in this case is going to represent the URL the request is going to and we are going to cache the get request we are not interested in caching that is not a get request

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService)

  const generateCacheKey = (url: string, params: HttpParams): string => {
    const paramsString = params.keys().map(key => `${key} = ${params.get(key)}`).join('&'); //the key will contain all the filters we are passing as of now.
    return paramsString ? `${url}?${paramsString}` : url;
  }

  const cachedKey = generateCacheKey(req.url, req.params);

  if(req.method === 'GET'){
    const cachedResponse = cache.get(cachedKey);
    if(cachedResponse){
      return of(cachedResponse);
    }
  }
  busyService.busy();

  return next(req).pipe(
    delay(500), 
    tap(response => {
      cache.set(cachedKey, response)
    }),
    finalize(() => {
      busyService.idel();
    })
  )
};
