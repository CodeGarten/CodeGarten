using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trac
{
    public enum TracPrivileges
    {
        BROWSER_VIEW,
        CHANGESET_VIEW,
        CONFIG_VIEW,
        EMAIL_VIEW,
        FILE_VIEW,
        LOG_VIEW,
        MILESTONE_ADMIN,
        MILESTONE_CREATE,
        MILESTONE_DELETE,
        MILESTONE_MODIFY,
        MILESTONE_VIEW,
        PERMISSION_ADMIN,
        PERMISSION_GRANT,
        PERMISSION_REVOKE,
        REPORT_ADMIN,
        REPORT_CREATE,
        REPORT_DELETE,
        REPORT_MODIFY,
        REPORT_SQL_VIEW,
        REPORT_VIEW,
        ROADMAP_ADMIN,
        ROADMAP_VIEW,
        SEARCH_VIEW,
        TICKET_ADMIN,
        TICKET_APPEND,
        TICKET_CHGPROP,
        TICKET_CREATE,
        TICKET_EDIT_CC,
        TICKET_EDIT_COMMENT,
        TICKET_EDIT_DESCRIPTION,
        TICKET_MODIFY,
        TICKET_VIEW,
        TIMELINE_VIEW,
        TRAC_ADMIN,
        VERSIONCONTROL_ADMIN,
        WIKI_ADMIN, WIKI_CREATE,
        WIKI_DELETE,
        WIKI_MODIFY,
        WIKI_RENAME, 
        WIKI_VIEW
    }
}
