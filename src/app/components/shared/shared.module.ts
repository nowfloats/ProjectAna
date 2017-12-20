import { ChatServerManagerComponent } from './chat-server-manager/chat-server-manager.component';
import { InfoDialogComponent } from './info-dialog/info-dialog.component';
import { LoadingIndicatorComponent } from './loading-indicator/loading-indicator.component';
import { LoginComponent } from './login/login.component';
import { PublishDialogComponent } from './publish-dialog/publish-dialog.component';

import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
	MatButtonModule,
	MatMenuModule,
	MatSidenavModule,
	MatInputModule,
	MatDialogModule,
	MatSelectModule,
	MatCheckboxModule,
	MatTabsModule,
	MatIconModule,
	MatTooltipModule,
	MatExpansionModule,
	MatProgressSpinnerModule,
	MatSnackBarModule,
	MatCardModule,
	MatListModule,
	MatGridListModule,
	MatProgressBarModule
} from '@angular/material';
import { EllipsisPipe } from '../../pipes/ellipsis.pipe';

import { AppComponent } from '../../app.component';

import { ChatFlowService } from '../../services/chatflow.service';
import { GlobalsService } from '../../services/globals.service';
import { SettingsService } from '../../services/settings.service';
import { InfoDialogService } from '../../services/info-dialog.service';
import { SimulatorService } from '../../services/simulator.service';

import { ManageUsersModule, MANAGE_USERS_ROUTES } from '../../components/manage-users/manage-users.module';
import { StudioModule, STUDIO_ROUTES } from '../../components/studio/studio.module';

const IMPORT_EXPORT = [
	HttpModule,
	FormsModule,
	MatButtonModule,
	MatMenuModule,
	MatSidenavModule,
	MatInputModule,
	MatDialogModule,
	HttpClientModule,
	MatSelectModule,
	MatCheckboxModule,
	MatTabsModule,
	MatIconModule,
	MatTooltipModule,
	MatExpansionModule,
	MatProgressSpinnerModule,
	MatProgressBarModule,
	MatSnackBarModule,
	MatCardModule,
	MatListModule,
	MatGridListModule,
	CommonModule
]

@NgModule({
	declarations: [
		ChatServerManagerComponent,
		InfoDialogComponent,
		LoadingIndicatorComponent,
		LoginComponent,
		PublishDialogComponent,
	],
	imports: (<any>[
		BrowserModule,
		BrowserAnimationsModule,
	]).concat(IMPORT_EXPORT),
	exports: (<any>[
		ChatServerManagerComponent,
		InfoDialogComponent,
		LoadingIndicatorComponent,
		LoginComponent,
		PublishDialogComponent
	]).concat(IMPORT_EXPORT),
	providers: [
		ChatFlowService,
		GlobalsService,
		SettingsService,
		InfoDialogService,
		Title,
		SimulatorService
	],
	schemas: [
		NO_ERRORS_SCHEMA
	],
	entryComponents: [
		InfoDialogComponent
	]
})
export class SharedModule { }