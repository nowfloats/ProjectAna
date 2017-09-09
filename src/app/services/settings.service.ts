import { Injectable } from '@angular/core';
import { ChatServerConnection } from '../models/app.models'

@Injectable()
export class SettingsService {
    constructor() { }

    loadSavedConnections() {
        var loaded = JSON.parse(localStorage.getItem(SettingKey.SavedConnsKey)) as ChatServerConnection[];
        if (loaded)
            return loaded;
        else
            return [];
    }

    saveSavedConnections(connections: ChatServerConnection[]) {
        localStorage.setItem(SettingKey.SavedConnsKey, JSON.stringify(connections));
    }
}

enum SettingKey {
    SavedConnsKey = 'SAVED_CONNECTIONS'
}
