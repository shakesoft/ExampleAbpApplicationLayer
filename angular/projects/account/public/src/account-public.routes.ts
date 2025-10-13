import {
  RouterOutletComponent,
  ReplaceableRouteContainerComponent,
  ReplaceableComponents,
  authGuard,
} from '@abp/ng.core';
import { Provider } from '@angular/core';
import { Routes } from '@angular/router';
import {
  LoginComponent,
  RegisterComponent,
  ForgotPasswordComponent,
  ResetPasswordComponent,
  EmailConfirmationComponent,
  LinkLoggedComponent,
  SendSecurityCodeComponent,
  LoginWithRecoveryCodeComponent,
  ManageProfileComponent,
  MySecurityLogsComponent,
  RefreshPasswordComponent,
  AccountSessionsComponent,
  ConfirmUserComponent,
} from './components';
import { eAccountRouteNames } from '@volo/abp.ng.account/public/config';
import {
  authenticationFlowGuard,
  securityCodeGuard,
  recoveryCodeGuard,
  confirmUserGuard,
} from './guards';
import { AccountConfigOptions } from './models';
import { ManageProfileResolver } from './resolvers';
import { accountExtensionsResolver } from './resolvers/extensions.resolver';
import { eAccountComponents } from './enums/components';
import { ACCOUNT_CONFIG_OPTIONS } from './tokens';
import {
  ACCOUNT_ENTITY_ACTION_CONTRIBUTORS,
  ACCOUNT_TOOLBAR_ACTION_CONTRIBUTORS,
  ACCOUNT_ENTITY_PROP_CONTRIBUTORS,
  ACCOUNT_EDIT_FORM_PROP_CONTRIBUTORS,
} from './tokens/extensions.token';
import { accountOptionsFactory } from './utils';
import { SecurityCodeService, RecoveryCodeService, ConfirmUserService } from './services';

export function createRoutes(config: AccountConfigOptions = {}): Routes {
  const accountProviders = [
    ManageProfileResolver,
    SecurityCodeService,
    RecoveryCodeService,
    ConfirmUserService,
  ];
  return [
    { path: '', pathMatch: 'full', redirectTo: 'login' },
    {
      path: '',
      component: RouterOutletComponent,
      providers: [...accountProviders, provideAccountContributors(config)],
      resolve: [accountExtensionsResolver],
      children: [
        {
          path: 'login',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authenticationFlowGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.Login,
              defaultComponent: LoginComponent,
            } as ReplaceableComponents.RouteData<LoginComponent>,
          },
          title: 'AbpAccount::Login',
        },
        {
          path: 'register',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authenticationFlowGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.Register,
              defaultComponent: RegisterComponent,
            } as ReplaceableComponents.RouteData<RegisterComponent>,
          },
          title: 'AbpAccount::Register',
        },
        {
          path: 'forgot-password',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authenticationFlowGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.ForgotPassword,
              defaultComponent: ForgotPasswordComponent,
            } as ReplaceableComponents.RouteData<ForgotPasswordComponent>,
          },
          title: 'AbpAccount::ForgotPassword',
        },
        {
          path: 'reset-password',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authenticationFlowGuard],
          data: {
            tenantBoxVisible: false,
            replaceableComponent: {
              key: eAccountComponents.ResetPassword,
              defaultComponent: ResetPasswordComponent,
            } as ReplaceableComponents.RouteData<ResetPasswordComponent>,
          },
          title: 'AbpAccount::ResetPassword',
        },
        {
          path: 'email-confirmation',
          component: ReplaceableRouteContainerComponent,
          data: {
            tenantBoxVisible: false,
            replaceableComponent: {
              key: eAccountComponents.EmailConfirmation,
              defaultComponent: EmailConfirmationComponent,
            } as ReplaceableComponents.RouteData<EmailConfirmationComponent>,
          },
          title: 'AbpAccount::EmailConfirmation',
        },
        {
          path: 'link-logged',
          component: ReplaceableRouteContainerComponent,
          data: {
            tenantBoxVisible: false,
            replaceableComponent: {
              key: eAccountComponents.LinkLogged,
              defaultComponent: LinkLoggedComponent,
            } as ReplaceableComponents.RouteData<LinkLoggedComponent>,
          },
          title: 'AbpAccount::LinkLogged',
        },
        {
          path: 'send-security-code',
          component: ReplaceableRouteContainerComponent,
          canActivate: [securityCodeGuard],
          data: {
            tenantBoxVisible: false,
            replaceableComponent: {
              key: eAccountComponents.SendSecurityCode,
              defaultComponent: SendSecurityCodeComponent,
            } as ReplaceableComponents.RouteData<SendSecurityCodeComponent>,
          },
          title: 'AbpAccount::TwoFactorVerification',
        },
        {
          path: 'login-with-recovery-code',
          component: ReplaceableRouteContainerComponent,
          canActivate: [recoveryCodeGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.LoginWithRecoveryCodeComponent,
              defaultComponent: LoginWithRecoveryCodeComponent,
            } as ReplaceableComponents.RouteData<LoginWithRecoveryCodeComponent>,
          },
          title: 'AbpAccount::LoginWithRecoveryCode',
        },
        {
          path: 'manage',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authGuard],
          resolve: {
            manageProfile: ManageProfileResolver,
          },
          data: {
            replaceableComponent: {
              key: eAccountComponents.ManageProfile,
              defaultComponent: ManageProfileComponent,
            } as ReplaceableComponents.RouteData<ManageProfileComponent>,
          },
          title: 'AbpAccount::MyAccount',
        },
        {
          path: 'security-logs',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.MySecurityLogs,
              defaultComponent: MySecurityLogsComponent,
            } as ReplaceableComponents.RouteData<MySecurityLogsComponent>,
          },
          title: 'AbpAccount::MySecurityLogs',
        },
        {
          path: 'change-password',
          component: ReplaceableRouteContainerComponent,
          data: {
            replaceableComponent: {
              key: eAccountComponents.RefreshPassword,
              defaultComponent: RefreshPasswordComponent,
            } as ReplaceableComponents.RouteData<RefreshPasswordComponent>,
          },
          title: 'AbpAccount::ChangePassword',
        },
        {
          path: 'sessions',
          component: ReplaceableRouteContainerComponent,
          canActivate: [authGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.Sessions,
              defaultComponent: AccountSessionsComponent,
            } as ReplaceableComponents.RouteData<AccountSessionsComponent>,
          },
          title: eAccountRouteNames.Sessions,
        },
        {
          path: 'confirm-user',
          component: ReplaceableRouteContainerComponent,
          canActivate: [confirmUserGuard],
          data: {
            replaceableComponent: {
              key: eAccountComponents.ConfirmUser,
              defaultComponent: ConfirmUserComponent,
            } as ReplaceableComponents.RouteData<ConfirmUserComponent>,
          },
          title: eAccountRouteNames.ConfirmUser,
        },
      ],
    },
  ];
}

function provideAccountContributors(options: AccountConfigOptions = {}): Provider[] {
  return [
    { provide: ACCOUNT_CONFIG_OPTIONS, useValue: options },
    {
      provide: 'ACCOUNT_OPTIONS',
      useFactory: accountOptionsFactory,
      deps: [ACCOUNT_CONFIG_OPTIONS],
    },
    {
      provide: ACCOUNT_ENTITY_ACTION_CONTRIBUTORS,
      useValue: options.entityActionContributors,
    },
    {
      provide: ACCOUNT_TOOLBAR_ACTION_CONTRIBUTORS,
      useValue: options.toolbarActionContributors,
    },
    {
      provide: ACCOUNT_ENTITY_PROP_CONTRIBUTORS,
      useValue: options.entityPropContributors,
    },
    {
      provide: ACCOUNT_EDIT_FORM_PROP_CONTRIBUTORS,
      useValue: options.personelInfoEntityPropContributors,
    },
  ];
}
