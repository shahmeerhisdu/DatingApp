import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/errors/not-found/not-found';
import { ServerError } from '../shared/errors/server-error/server-error';
import { MemberProfile } from '../features/members/member-profile/member-profile';
import { MemberPhotos } from '../features/members/member-photos/member-photos';
import { MemberMessages } from '../features/members/member-messages/member-messages';
import { memberResolver } from '../features/members/member-resolver';
import { preventUnsavedChangesGuard } from '../core/guards/prevent-unsaved-changes-guard';
import { Admin } from '../features/admin/admin';
import { adminGuard } from '../core/guards/admin-guard';

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
                resolve: {member: memberResolver},
                runGuardsAndResolvers: 'always',
                component: MemberDetailed,
                children: [
                    {
                        path: '',
                        redirectTo: 'profile', //means when someone comes to the path members/:id will be redirected to the profile
                        pathMatch: 'full'// we have two options for the path matching full and the prefix, and defualt is the prefix and by default the router checks the URL elements from the left and if the URL matches the given path and stops when there is a config match. By full we are overriding this default behaviour so that the URL should not stop here path: '' else this will gonna match members/:id when we are going to one of our routes inside here. so we want to match the path fully other than just the prefix otherwise we will never be able to get to the other components because we will always be redirected to profile.
                    },
                    {
                        path: 'profile',
                        component: MemberProfile,
                        title: 'Profile', // title on the page will be updated based on the routes provided.
                        canDeactivate: [preventUnsavedChangesGuard]
                    },
                    {
                        path: 'photos',
                        component: MemberPhotos,
                        title: 'Photos'
                    },
                    {
                        path: 'messages',
                        component: MemberMessages,
                        title: 'Messages'
                    }
                ]
            },
            {
                path: "lists",
                component: Lists
            },
            {
                path: "messages",
                component: Messages
            },
            {
                path: "admin",
                component: Admin,
                canActivate: [adminGuard]
            },
        ]
    },
    {
        path: "errors",
        component: TestErrors
    },
    {
        path: "server-error",
        component: ServerError
    },
    {
        path: "**", /** wild card route we will change component to Not Found */
        component: NotFound
    }
];
