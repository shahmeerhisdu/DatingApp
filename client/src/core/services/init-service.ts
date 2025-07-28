import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InitService {

  private accountService = inject(AccountService)

  init(){

    //in reality it should return something so that we should know that user is logged in, to return async code in angular we use the observeables.
    const userString = localStorage.getItem('user');
    if(!userString) return of(null);

    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user)

    return of(null); //this is observeable from rxjs, this used to be called observeable of but they have shortened this to only of.
  }
  constructor() { }
}
