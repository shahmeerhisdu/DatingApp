import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { LikesService } from '../../../core/services/likes-service';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-members-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './members-card.html',
  styleUrl: './members-card.css'
})
export class MembersCard {
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService)
  member = input.required<Member>();

  // we can create a computed property to check if the current user has liked the user of the member card that we are looking at.
  protected hasLiked = computed(() => this.likeService.likedIds().includes(this.member().id))
  protected isOnline = computed(() => this.presenceService.onlineUsers().includes(this.member().id));

  toggleLike(event: Event){
    event.stopPropagation(); //to prevent the click event from routing to user detail page.
    this.likeService.toggleLike(this.member().id)
  }

}
