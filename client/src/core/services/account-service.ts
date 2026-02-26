import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';
import { PresenceService } from './presence-service';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root' //this means that it is automatically provided to our application as a whole
})

// this injectable specifies that this class can use the dependency injection from the angular and can be injected into other components or services or classes in our angular project.
export class AccountService {

  //services are both injectable and they are considered singleton, components are different and components are distroyed when they are outside of the scope, this angular service is started when our angular application starts and its gonna be a singleton and survives as long as our application does. So this is the good place to store the state that we need application wide, many components might need the access to the same property, the components can come in and come out of the scope but the angular service will survive as the singleton and will always be available. So Service is the good place for the state and also for the data fetching.

  // constructor() { } we don't need the constructor as now the recommendation from the angular is to use the inject function.

  private http = inject(HttpClient);
  private likeService = inject(LikesService); //don't inject AccountService into LikesService and LikesService into AccountService as this will create circular dependency and will break the application.
  private presenceService = inject(PresenceService)
  currentUser = signal<User | null>(null)

  private baseUrl = environment.apiUrl;

  register(creds: RegisterCreds) {
    return this.http.post<User>(this.baseUrl + 'account/register', creds, { withCredentials: true }).pipe(
      // tap function allows us to use a side effect on what we get back from our API without actually modifying the data
      tap(user => {
        if (user) {
          this.setCurrentUser(user)
          this.startTokenRefreshInterval(); //start the token refresh interval when we register a new user, this will ensure that the token is refreshed automatically when it expires, and we don't have to worry about it in the rest of our application.
        }
      })
    )
  }

  login(creds: LoginCreds) {
    return this.http.post<User>(this.baseUrl + 'account/login', creds, { withCredentials: true }).pipe(
      // tap function allows us to use a side effect on what we get back from our API without actually modifying the data
      tap(user => {
        debugger
        if (user) {
          this.setCurrentUser(user)
          this.startTokenRefreshInterval(); //start the token refresh interval when we login, this will ensure that the token is refreshed automatically when it expires, and we don't have to worry about it in the rest of our application.
        }
      })
    )
  }

  refreshToken() {
    debugger
    return this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, { withCredentials: true })
  }

  startTokenRefreshInterval() {
    setInterval(() => {
      this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, { withCredentials: true }).subscribe(
        {
          next: user => {
            this.setCurrentUser(user);
          }, //this will issue the new token to us
          error: () => {
            this.logout(); //if we fail to refresh the token, we will log out the user, this can happen when the refresh token expires or is invalid for some reason, so we want to log out the user in that case.
          }
        }
      )
    }, 5 * 60 * 1000) // Refresh token every 5 minutes
  }

  setCurrentUser(user: User) {
    user.roles = this.getRolesFromToken(user);
    this.currentUser.set(user)
    this.likeService.getLikedIds(); //this will populate the likedIds signal in the LikesService when we set the current user, and we can use that signal across the application to show which members are liked by the current user.

    if (this.presenceService.hubConnection?.state !== HubConnectionState.Connected) {
      this.presenceService.createHubConnection(user);
    }
  }

  logout() {
    this.http.post(this.baseUrl + 'account/logout', {}, { withCredentials: true }).subscribe({
      next: () => {
        localStorage.removeItem('filters')
        this.currentUser.set(null)
        this.likeService.clearLikedIds(); //clear the likedIds signal in the LikesService when we logout.
        this.presenceService.stopHubConnection();
      }
    })//using with credentials because we want our cookies to be removed

  }

  private getRolesFromToken(user: User): string[] {
    const payload = user.token.split('.')[1];
    const decodedPayload = atob(payload);//atob is a built-in function in JavaScript that decodes a base-64 encoded string.
    const tokenData = JSON.parse(decodedPayload);
    return Array.isArray(tokenData.role) ? tokenData.role : [tokenData.role];
  }
}
