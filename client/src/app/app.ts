import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { Home } from "../features/home/home";
import { User } from '../types/user';
import { Router, RouterOutlet } from '@angular/router';
import { ConfirmDialog } from "../shared/confirm-dialog/confirm-dialog";

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, ConfirmDialog],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

//to make tha HTTP request we first need to understand the component life cycle of Angular APP but before we get into this we need to make the HTTP client module make availabe to our APP

//Component Life Cycle
// 1:  SO any JS or TS class can have the constructor, this is the first event that happens when the component is instanciated or a class is instanciated the code inside the constructor is instanciated, so we need to inject the HTTP client to this file using the constructor, sot the constructor is the first thing that happens if provided.

//2: next step after the class is constructed is that this class is initialized and we can do something in the initialization of the component and inorder to do so we implement an interface and the one we are going to implement is oninit interface that we get from the angular core 

//and this is the dependency injection, but this is the old method now we have the inject from the angular to do so, that I have done on the top.
// constructor(private http: HttpClient) {
// we don't fetch the data in the constructor while the class is constructoring we do this when the class is instanciated that is in the ngOnInit() method 

// }


export class App {
  // private accountService = inject(AccountService);
  protected router = inject(Router);
  // private http = inject(HttpClient);
  // protected title = 'Learning APP';
  // protected members: any;
  // protected members = signal<User[]>([]) //this is fine grained change detection, so when the value of this signal changes then anything that is using this signal will be notified.


  // ngOnInit(): void {
  //   //this http get method returns an observeable and the observeable is a tool for managing an asynchronous data streams and it represents the stream of data that can emitt multiple values over time, but that not what we are doing over http request for now, so if we want to observe we have to subscribe to it.

  //   // if the cors is not enabled we will get the CORS error in the console, but 200 ok response from the server because server has retured the data but its the browers functionality that it doesn't allow us to use the data if there is nothing in the header in the request.

  //   //as our server is running on different domain and we are accessing the data from different domain so browser secuirty that is the CORS don't allow to do so untill the data sent from the server side has domain in the header from which we are accessing the data.

  //   //for now if we refresh the browser we will not see the members again although the request is completed, because we have created the zoneless application and thats what the zonelesschange detection means in the app.config.ts, that means that angular is not using the zonejs javascript to monitor changes in our code or in the user interface. So this is the future and how to tackle this by using the angular signals 

  //   //so change the type of members to angular signal

  //   //now incase the request is stuck and is not complete and no errors then it will not be unsubscribed, so I am writing a new ngoninit
  //   this.http.get('https://localhost:5001/api/members').subscribe({
  //     // next: respone => this.members = respone,
  //     next: respone => this.members.set(respone),
  //     error: error => console.log(error),
  //     complete: () => console.log("Comopleted the http request")

  //   })
  // }

  // async ngOnInit() {
  //   // this.setCurrentUser();
  //   this.members.set(await this.getMembers())
  // }

  // setCurrentUser() {
  //   const userString = localStorage.getItem('user');
  //   if(!userString) return;

  //   const user = JSON.parse(userString);
  //   this.accountService.currentUser.set(user)
  // }
  // async getMembers() {
  //   try {
  //     return lastValueFrom(this.http.get<User[]>('https://localhost:5001/api/members'));
  //   } catch (error) {
  //     console.log(error)
  //     throw error
  //   }
  // }

}
