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

  const invalidateCache = (urlPattern: string) => {
    // we are effectively going to loop over our cached keys and we are going to look for the pattern that maches the Url inside our cache keys and if it finds it based on what we have passed to this method then we are going to delete that cache key. And now when the next request comes in for that URL it is going to be a cache miss and it is going to go to the server to get the latest data.
    for(const key of cache.keys()){
      if(key.includes(urlPattern)){
        cache.delete(key);
        console.log(`Cache invalidated for key: ${key}`);
      }
    }
  }

  const cachedKey = generateCacheKey(req.url, req.params);

  if(req.method.includes('POST') && req.url.includes('/likes')){
    invalidateCache('/likes');
  }

  // We are doing this for all of the get requests. But we need a process where we can invalidate this caching so that when a user do something like, liking another user, the cache is invalidated and the new data is fetched from the server instead of the cache.
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
