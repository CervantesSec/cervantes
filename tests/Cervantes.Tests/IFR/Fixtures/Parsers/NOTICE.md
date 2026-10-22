# Parser fixtures

Every file here is real output of the tool it is named after, trimmed by deleting whole findings. Nothing in a kept finding was edited, except where a line below says otherwise.

## Output generated for Cervantes

Produced on 2026-10-22 by running the tools in Docker against throwaway local containers (`httpd:2.4.49` and `python:3.12-alpine` `http.server`) on an isolated Docker network:

| Folder | Tool and version |
|---|---|
| `Trivy` | Trivy 0.58.1 (`trivy image --format json httpd:2.4.49`) |
| `Nuclei` | Nuclei v3.11.1, templates v10.5.0 (`-jsonl`, `-je`) |
| `Bandit` | Bandit 1.8.0 (`bandit -r src -f json`) |
| `Nikto` | Nikto 2.5.0 and Nikto master 312645d (reports 2.6.1) (`-Format json`, `-Format xml`) |
| `Masscan` | Masscan 1.3.2, Alpine 3.20 package (`-oJ`, `-oX`, `-oL`, with and without `--banners`) |
| `Zap` | ZAP 2.17.0 (`zap-baseline.py -x`) |

Files whose structure was copied from a public report but whose content is ours (the source is not redistributed):

- `Prowler/prowler-v3.csv`: header and column layout of a Prowler v3 CSV report (`;`-separated); rows written for Cervantes.
<!-- filled in by the tasks that add them -->

## Files from DefectDojo

The files listed below come from the test samples of DefectDojo (`https://github.com/DefectDojo/django-DefectDojo`, folder `unittests/scans`), trimmed as described above. They are distributed under the following license:

- `Prowler/prowler-v3.json`: `aws_prowler_v3plus/many_vuln.json`
- `Prowler/prowler-v4.ocsf.json`: `prowler/example_output_aws.ocsf.json`
- `Prowler/prowler-v4.csv`: `prowler/example_output_aws.csv`
- `Qualys/qualys-scan.xml`: `qualys_infrascan_webgui/qualys_infrascan_webgui_3.xml`
- `Qualys/qualys-scan-multiple.xml`: `qualys_infrascan_webgui/qualys_infrascan_webgui_multiple.xml`
- `Qualys/qualys-asset-data.xml`: `qualys/empty.xml`
- `Qualys/qualys-was.xml`: `qualys_webapp/qualys_webapp_one_vuln.xml`
- `Acunetix/acunetix.xml`: `acunetix/XML_http_example_co_id_.xml` (report items 1, 5 and 6 and the first two crawler site files kept)
- `Acunetix/acunetix-many.xml`: `acunetix/many_findings_with_port_number.xml`
- `Acunetix/acunetix360.json`: `acunetix/acunetix360_one_finding.json`
- `Nessus/nessus-cvssv3.nessus`: `tenable/nessus/nessus_with_cvssv3.nessus` (report items 4, 5, 13 and 17, the first two plugin preference items, and no `plugin_set` preference)
- `OpenVAS/openvas.xml`: `openvas/report_detail_v2.xml`
- `OpenVAS/openvas-gmp9.xml`: `openvas/one_vuln.xml`
<!-- filled in by the tasks that add them -->

> Copyright (c) 2015-2023 DefectDojo, Inc. All rights reserved.
>
> Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
>
> 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
>
> 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
>
> 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
>
> THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
