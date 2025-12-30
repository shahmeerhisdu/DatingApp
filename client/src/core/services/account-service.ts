import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';

@Injectable({
  providedIn: 'root' //this means that it is automatically provided to our application as a whole
})

// this injectable specifies that this class can use the dependency injection from the angular and can be injected into other components or services or classes in our angular project.
export class AccountService {

  //services are both injectable and they are considered singleton, components are different and components are distroyed when they are outside of the scope, this angular service is started when our angular application starts and its gonna be a singleton and survives as long as our application does. So this is the good place to store the state that we need application wide, many components might need the access to the same property, the components can come in and come out of the scope but the angular service will survive as the singleton and will always be available. So Service is the good place for the state and also for the data fetching.

  // constructor() { } we don't need the constructor as now the recommendation from the angular is to use the inject function.

  private http = inject(HttpClient);
  private likeService = inject(LikesService); //don't inject AccountService into LikesService and LikesService into AccountService as this will create circular dependency and will break the application.
  currentUser = signal<User | null>(null)

  private baseUrl = environment.apiUrl;

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe(
      // tap function allows us to use a side effect on what we get back from our API without actually modifying the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
        }
      })
    )
  }

  login(creds: LoginCreds) {
    return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe(
      // tap function allows us to use a side effect on what we get back from our API without actually modifying the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
        }
      })
    )
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user))
    this.currentUser.set(user)
    this.likeService.getLikedIds(); //this will populate the likedIds signal in the LikesService when we set the current user, and we can use that signal across the application to show which members are liked by the current user.
  }

  logout() {
    localStorage.removeItem('user')
    localStorage.removeItem('filters')
    this.currentUser.set(null)
    this.likeService.clearLikedIds(); //clear the likedIds signal in the LikesService when we logout.
  }
}
