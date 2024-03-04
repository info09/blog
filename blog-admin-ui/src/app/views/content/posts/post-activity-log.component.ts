import { AdminApiPostApiClient, PostActivityLogDto } from './../../../api/admin-api.service.generated';
import { UtilityService } from './../../../shared/services/utility.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
@Component({
    templateUrl: 'post-activity-log-component.html'
})
export class PostActivityLogComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();

    // Default
    public blockedPanel: boolean = false;
    public title: string;
    public items: any[] = [];

    constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig, private utilService: UtilityService, private postService: AdminApiPostApiClient) {

    }
    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.toggleBlockUI(true);
        this.postService.getActivityLogs(this.config.data?.id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: PostActivityLogDto[]) => {
                this.items = res;
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }
    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.blockedPanel = true;
        }
        else {
            setTimeout(() => {
                this.blockedPanel = false;
            }, 1000);
        }
    }
}