import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  private accountService = inject(AccountService)

  init() {
    
    return this.accountService.refreshToken().pipe(
      tap(user => {
        debugger
        if (user) {
          debugger
          this.accountService.setCurrentUser(user)
          this.accountService.startTokenRefreshInterval(); //start the token refresh interval when the application initializes if there is a logged in user, this will ensure that the token is refreshed automatically when it expires, and we don't have to worry about it in the rest of our application.
        }
      })
    )
  }
  constructor() { }
}
