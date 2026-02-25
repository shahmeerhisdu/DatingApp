import { Component, effect, ElementRef, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MessageService } from '../../../core/services/message-service';
import { MemberService } from '../../../core/services/member-service';
import { Message } from '../../../types/message';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormsModule } from '@angular/forms';
import { PresenceService } from '../../../core/services/presence-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-messages',
  imports: [DatePipe, TimeAgoPipe, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css'
})
export class MemberMessages implements OnInit, OnDestroy {
  @ViewChild('messageEndRef') messageEndRef!: ElementRef;

  protected messageService = inject(MessageService);
  private memberService = inject(MemberService);
  protected presenceService = inject(PresenceService);
  private route = inject(ActivatedRoute); //this we need because we want to get the userId of the other user from the route parameter
  protected messageContent = '';

  //We need to create the hub connection inside here because when this component is loaded thats when we want to connect to the signalR hub.


  constructor() {
    effect(() => {
      //this effect will run whenever the messages signal changes
      const currentMessages = this.messageService.messageThread();

      if (currentMessages.length > 0)
        this.scrollToBottom();
    });
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe({
      next: params => {
        const otherUserId = params.get('id');
        if (!otherUserId) {
          throw new Error('Can not connect to the hub at this point');
        }
        this.messageService.createHubConnection(otherUserId)
      }
    })
  }


  sendMessage() {
    const recipientId = this.memberService.member()?.id;
    if (!recipientId) return;
    this.messageService.sendMessage(recipientId, this.messageContent)?.then(() => {
      this.messageContent = '';
    })
  }

  scrollToBottom(): void {

    //we need to call this method when the component has rendered, or after the JS call stack is clear.
    setTimeout(() => {
      if (this.messageEndRef) {
        this.messageEndRef.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    });

  }
}
