import { Component, input } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';
import { AgePipe } from '../../../core/pipes/age-pipe';

@Component({
  selector: 'app-members-card',
  imports: [RouterLink, AgePipe],
  templateUrl: './members-card.html',
  styleUrl: './members-card.css'
})
export class MembersCard {
  member = input.required<Member>();
}
