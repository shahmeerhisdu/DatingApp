import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError } from 'rxjs';
import { ToastService } from '../services/toast-service';
import { NavigationExtras, Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  // we have http req and next method so that we can send this to next destination
  // next is the observable but we have to do something with this observable so we have to catch the error using the rxjs

  const toast = inject(ToastService);
  const router = inject(Router); // to redirect the user to not found page
  return next(req).pipe(
    catchError(error => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              const modelStateErrors = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modelStateErrors.push(error.error.errors[key]);
                }
              }
              throw modelStateErrors.flat()

            } else {
              toast.error(error.error);
            }
            break;
          case 401:
            toast.error('Unauthorized');
            break;
          case 404:
            router.navigateByUrl('/not-found')
            break;
          case 500:
            const navigationExtras : NavigationExtras = {state: {error: error.error}};
            router.navigateByUrl('/server-error', navigationExtras)
            break;
          default:
            toast.error('Something went wrong');
            break;
        }
      }
      throw error;
    })
  )
};
