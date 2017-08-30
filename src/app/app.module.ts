import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MdButtonModule, MdMenuModule, MdSidenavModule, MdInputModule, MdDialogModule } from '@angular/material';

import { AppComponent } from './app.component';
import { ChatFlowComponent } from './components/chatflow/chatflow.component';
import { NodeEditorComponent } from './components/nodeeditor/nodeeditor.component';

import { ChatFlowService } from './services/chatflow.service';

@NgModule({
  declarations: [
    AppComponent,
    ChatFlowComponent,
    NodeEditorComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    HttpModule,
    FormsModule,
    MdButtonModule,
    MdMenuModule,
    MdSidenavModule,
    MdInputModule,
    MdDialogModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'designer', pathMatch: 'full' },
      { path: 'designer', component: ChatFlowComponent },
      { path: 'editor', component: NodeEditorComponent },
      { path: '**', redirectTo: 'designer' }
    ])
  ],
  providers: [
    ChatFlowService
  ],
  bootstrap: [AppComponent],
  schemas: [
    NO_ERRORS_SCHEMA
  ]
})
export class AppModule { }
