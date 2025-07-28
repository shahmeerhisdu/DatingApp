import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';

export const routes: Routes = [
    {
        path: "",
        component: Home
    },
    {
        path: "",
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            {
                path: "members",
                component: MemberList,
            },
            {
                path: "members/:id", //:id dynamic root paramter we specify : for this
                component: MemberDetailed
            },
            {
                path: "lists",
                component: Lists
            },
            {
                path: "messages",
                component: Messages
            },
        ]
    },

    {
        path: "**", /** wild card route we will change component to Not Found */
        component: Home
    }
];
