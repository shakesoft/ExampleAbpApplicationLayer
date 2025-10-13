import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { ModuleWithProviders, NgModule, NgModuleFactory } from '@angular/core';
import { ChatRoutingModule } from './chat-routing.module';
import { ChatContactsComponent } from './components/chat-contacts.component';
import { ChatComponent } from './components/chat.component';
import { ConversationAvatarComponent } from './components/conversation-avatar.component';
import { ChatMessageTitleComponent } from './components/chat-message-title.component';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

const declarations = [
  ChatComponent,
  ChatContactsComponent,
  ConversationAvatarComponent,
  ChatMessageTitleComponent,
];

@NgModule({
  imports: [CoreModule, ThemeSharedModule, ChatRoutingModule, NgbDropdownModule, ...declarations],
  exports: [...declarations],
})
export class ChatModule {
  static forChild(): ModuleWithProviders<ChatModule> {
    return {
      ngModule: ChatModule,
      providers: [],
    };
  }
  /**
   * @deprecated `ChatModule.forLazy()` is deprecated. You can use `createRoutes` **function** instead.
   */
  static forLazy(): NgModuleFactory<ChatModule> {
    return new LazyModuleFactory(ChatModule.forChild());
  }
}
