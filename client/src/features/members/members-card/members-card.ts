import { Component, input } from '@angular/core';
import { Member } from '../../../types/member';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-members-card',
  imports: [RouterLink],
  templateUrl: './members-card.html',
  styleUrl: './members-card.css'
})
export class MembersCard {
  member = input.required<Member>();
}
