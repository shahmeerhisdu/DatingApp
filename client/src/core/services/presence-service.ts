import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToastService } from './toast-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../../types/user';
import { Message } from '../../types/message';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  private toast = inject(ToastService);
  hubConnection?: HubConnection
  onlineUsers = signal<string[]>([]);

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

    this.hubConnection.on('UserOnline', userId => {
      this.onlineUsers.update(users => [...users, userId]);
    })

    this.hubConnection.on('UserOffline', userId =>{
      this.onlineUsers.update(users => users.filter(x => x !== userId))
    })

    this.hubConnection.on('GetOnlineUsers', userIds => {
      this.onlineUsers.set(userIds);
    })

    this.hubConnection.on('NewMessageReceived', (message:Message) =>{
      this.toast.info(message.senderDisplayName + ' has sent you a new message');
    })
  }
  //now we will make the above connection when the user logs in, and on log out we will stop the connection.
  stopHubConnection(){
    if(this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }
}
