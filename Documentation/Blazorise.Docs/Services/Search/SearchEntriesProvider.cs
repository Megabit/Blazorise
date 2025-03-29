using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Blazorise.Docs.Models.ApiDocsDtos;
using Blazorise.Shared.Models;

namespace Blazorise.Docs.Services.Search;

public class SearchEntriesProvider
{
    public List<PageEntry> Entries => entries
                                      ?? throw new InvalidOperationException( "SearchEntriesProvider not initialized. Call InitializeAsync instead before using it." );

    private List<PageEntry> entries;
    public async ValueTask<List<PageEntry>> InitializeAsync()
    {
        if ( entries is not null )
            return entries;

        List<PageEntry> fromComponentEntries = [];

        var eligibleApiDocs = ComponentsApiDocsSource.Instance.Components
                                                     .Where( x => x.Value.SearchUrl is not null )
                                                     .Select( x => x.Value );
        foreach ( var comp in eligibleApiDocs )
        {
            //add component if not already in manualPageEntries. eg UnorderedList
            if ( ManualPageEntries.Entries.All( x => x.Name != comp.TypeName ) )
            {
                fromComponentEntries.Add( new PageEntry( comp.SearchUrl, comp.TypeName ) );
            }

            var entriesForComp = comp.AllApiDocsRecords
                                     .Select( x => PageEntry.GetDocsPageEntryForParams( comp, x ) );

            fromComponentEntries.AddRange( entriesForComp );
        }

        await Task.CompletedTask;
        entries = ManualPageEntries.Entries.Concat( fromComponentEntries ).ToList();
        return entries;
    }
}