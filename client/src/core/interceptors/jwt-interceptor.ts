import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../services/account-service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);

  const user = accountService.currentUser(); // here I have created the reference to the current user signal then I am making a copy of the value of that signal and this user does not have any reactivity when we use it like this, means its no loger a signal at this point. So you loose the reactivity when you create the copy of the signal by getting its value using this approach.

  //Now the request itself is immuteable, we can't directly modify it, we have to create the clone of it

  if(user){
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}`
      }
    })
  }

  return next(req);
};
