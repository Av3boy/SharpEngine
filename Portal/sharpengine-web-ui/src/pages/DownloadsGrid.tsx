import { Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material';
import CollapsableGrid from '../components/Grid/CollapsableGrid';
import type { Release } from './DownloadsPage';

export type DownloadsGridProps = {
  items: Release[];
};

export default function DownloadsGrid({ items }: DownloadsGridProps) {
  return (
    <CollapsableGrid<Release>
      items={items}
      renderHeaders={['Release', 'Tag', 'Published', 'Assets']}
      keyExtractor={(item) => `${item.id}`}
      renderSummaryCells={(item) => [
        item.name,
        item.tag,
        item.publishedAt ? new Date(item.publishedAt).toLocaleDateString() : '—',
        item.assets.length,
      ]}
      renderDetails={(item) => (
        <Table size="small" aria-label="assets">
          <TableHead>
            <TableRow>
              <TableCell>Asset</TableCell>
              <TableCell align="right">Size</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Download</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {item.assets.map((a) => (
              <TableRow key={a.id}>
                <TableCell>{a.name}</TableCell>
                <TableCell align="right">
                  {a.size ? `${(a.size / (1024 * 1024)).toFixed(1)} MB` : '—'}
                </TableCell>
                <TableCell>{a.contentType ?? '—'}</TableCell>
                <TableCell>
                  <a href={a.browserDownloadUrl} target="_blank" rel="noreferrer" className="text-blue-400 hover:text-blue-300">
                    Download
                  </a>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    />
  );
}