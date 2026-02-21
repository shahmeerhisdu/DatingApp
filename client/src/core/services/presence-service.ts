import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../../types/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  private toast = inject(ToastService);
  hubConnection?: HubConnection

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token // pass the token as query string parameter
      })
      .withAutomaticReconnect()
      .build();
    
    this.hubConnection.start()
      .catch(error => console.log(error));

    // now listen to the event of useronline and useroffline we craeted in the server presencehub. Name should exactly match.

    this.hubConnection.on('UserOnline', email => {
      this.toast.success(email + ' has connected');
    })

    this.hubConnection.on('UserOffline', email =>{
      this.toast.info(email + ' has disconnected');
    })

  }
  //now we will make the above connection when the user logs in, and on log out we will stop the connection.
  stopHubConnection(){
    if(this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }
}
