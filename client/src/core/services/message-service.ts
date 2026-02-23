import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Message } from '../../types/message';
import { PaginatedResult } from '../../types/pagination';
import { AccountService } from './account-service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';


@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = environment.apiUrl;
  //we will need few things to inject, for signalR we need to send the Token with the request to negociate the connection
  private hubUrl = environment.hubUrl;
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  private hubConnection? : HubConnection;
  // we also want to store here the meesage thread, because we will use the signal inside our message service so that when we do receive that information from the signalR hub we just update the message thread inside here with the signal so that our component can react to that change.
  messageThread = signal<Message[]>([]);

  createHubConnection(otherUserId: string){
    const currentUser = this.accountService.currentUser();
    if(!currentUser) return;

    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'messages?userId=' + otherUserId, {
      accessTokenFactory: () => currentUser.token
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection.start().catch(e => console.log("Error: ", e));

    //listen to the events from the signalR hub
    this.hubConnection.on('ReceivedMessageThread', (messages: Message[]) => {
      this.messageThread.set(messages.map(message => ({
            ...message,
            currentUserSender: message.senderId !== otherUserId
          })));
    })
  }

  stopHubConnection(){
    if(this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(e => console.log(e));
    }
  }

  getMessages(container: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params  = params.append('pageNumber', pageNumber);
    params  = params.append('pageSize', pageSize);
    params  = params.append('Container', container);

    return this.http.get<PaginatedResult<Message>>(`${this.baseUrl}messages`, { params });
  }

  getMessageThread(memberId: string) {
    return this.http.get<Message[]>(`${this.baseUrl}messages/thread/${memberId}`);
  }

  sendMessage(recipientId: string, content: string) {
    return this.http.post<Message>(`${this.baseUrl}messages`, { recipientId, content });
  }

  deleteMessage(id: string){
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
