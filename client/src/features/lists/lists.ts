import { Component, inject, OnInit, signal } from '@angular/core';
import { LikesService } from '../../core/services/likes-service';
import { Member } from '../../types/member';
import { MembersCard } from "../members/members-card/members-card";

@Component({
  selector: 'app-lists',
  imports: [MembersCard],
  templateUrl: './lists.html',
  styleUrl: './lists.css'
})
export class Lists implements OnInit {

  private likeService = inject(LikesService);
  protected members = signal<Member[]>([]);
  protected predicate = 'liked';

  tabs = [
    { label: 'Liked', value: 'liked' },
    { label: 'Liked Me', value: 'likedBy' },
    { label: 'Mutual', value: 'mutual' }
  ];

  ngOnInit(): void {
    this.loadLikes();
  }

  setPredicate(predicate: string){
    if(this.predicate !== predicate){
      this.predicate = predicate;
      this.loadLikes();
    }
  }

  loadLikes(){
    this.likeService.getLikes(this.predicate).subscribe({
      next: members => {
        this.members.set(members);
      }
    });    
  }
}
