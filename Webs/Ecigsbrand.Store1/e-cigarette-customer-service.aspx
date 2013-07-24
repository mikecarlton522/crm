<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/Front.Master" AutoEventWireup="true"
    CodeBehind="e-cigarette-customer-service.aspx.cs" Inherits="Ecigsbrand.Store1.e_cigarette_customer_service" %>

<%@ Register TagPrefix="uc" TagName="Accessories" Src="~/Controls/Accessories.ascx" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphScript" runat="server">
    <script>
        $(document).ready(function () {
            $("#accordion").accordion({ autoHeight: false, collapsible: true });
        });
  </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    E-Cigs: Electronic Cigarette Customer Service</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div id="content">
        <div class="left">
            <img src="images/customer-service-girl.jpg" width="150" height="150" align="right"
                style="border: 1px solid black; margin: 30px 0 0 15px;">
            <h1>
                Customer Service</h1>
            <p>
                Our entire team is committed to providing the highest level of customer support
                possible and we encourage you to contact us to discuss your order should you have
                any problems or concerns. Hundreds of customers are making the shift to a healthier
                lifestyle every day by taking advantage of the revolutionary electronic cigarette.
                Let’s work together to give you the healthier lifestyle you want.</p>
            <h2>
                Call us Toll-Free: <span class="red">1-866-830-2464</span></h2>
            <h3>
                UK Customer Service: <span class="red">44 (0) 2033 188 540.</span></h3>
            <h3>
                Monday 4AM - Saturday 10PM CST (Open 24 Hours)<br>
                Sundays: Closed</h3>
            <h3>
                Email us at <a href="mailto:support@ecigsbrand.com">support@ecigsbrand.com</a></h3>
            <p>
                While we strive for prompt support, please allow up to 24 hours for a response through
                email.</p>
            <img src="images/customer-service-start.jpg" width="150" height="150" align="right"
                style="border: 1px solid black; margin: 30px 0 0 15px;">
            <a name="guide"></a>
            <h1>
                Quick Start Guide</h1>
            <p>
                Every E-Cigs customer should read our Quick Start Guide to help familiarize them
                with our electronic cigarette.
                <h3>
                    <a href="http://www.ecigsbrand.com/getting-started-faq.pdf">Download PDF Now! About
                        3 MB</a></h3>
                <h3>
                    Are electronic cigarettes safe?</h3>
                <img src="images/customer-service-faq.jpg" width="150" height="150" align="right"
                    style="border: 1px solid black; margin: 30px 0 0 15px;">
                <a name="faq"></a>
                <h1>
                    Frequently Asked Questions</h1>
                <div id="accordion">
                    <h3>
                        <a href="#">Are electronic cigarettes safe?</a></h3>
                    <p>
                        While no use of nicotine comes without risks, electronic cigarettes are one of the
                        safest forms of nicotine available. Electronic cigarettes contain only water, propylene
                        glycol, nicotine, and flavoring that imitates tobacco flavor and have their cartridges
                        have been toxicologically tested and contains no known ingredients that are considered
                        cancer-causing agents.</p>
                    <h3>
                        <a href="#">Do I need a lighter for the electronic cigarette?</a></h3>
                    <p>
                        No, you do not need a lighter for the electronic cigarette as it’s a completely
                        electronic device. Do not try to light e-cigarette as it contains a lithium-ion
                        battery which could be explosive if ignited.</p>
                    <h3>
                        <a href="#">What is the smoke that is released from the electronic cigarette?</a></h3>
                    <p>
                        The smoke released from the electronic cigarette is simply a water vapor that evaporates
                        into the air within seconds and leaves no residue in the air without harming anyone
                        around you with the second-hand smoke of traditional cigarettes.</p>
                    <h3>
                        <a href="#">Does the electronic cigarette have a taste?</a></h3>
                    <p>
                        Yes, most definitely! The carefully crafted taste resembles that of traditional
                        tobacco, but users tend to find it smoother and cleaner as it lacks many other harsh
                        additives and burning tobacco of a real cigarette. Don’t forget to ask about our
                        menthol flavor.</p>
                    <h3>
                        <a href="#">Why do people switch to electronic cigarettes?</a></h3>
                    <p>
                        For similar reasons to why people smoke traditional cigarettes, smoking electronic
                        cigarettes provides a smilar sensation. They provide delight and relaxation not
                        to mention the social benefits from smoking with friends and colleagues. As well,
                        electronic cigarettes are much more convenient as they can be smoked anywhere, have
                        no risk of fire, leave no second-hand smoke and can be used an put away without
                        waste. Lastly, the electronic cigarette alternative is much more cost effective
                        as they cost approximately half that of traditional cigarettes.</p>
                    <h3>
                        <a href="#">Can I smoke anywhere?</a></h3>
                    <p>
                        Yes, our electronic cigarettes can be used legally almost anywhere, even where traditional
                        smoking is prohibited. Because the electronic cigarette is not lit and smoke is
                        not produced, it is not known to be prohibited from use under most laws and ordinances.
                        Please check your local jurisdiction for details.</p>
                    <h3>
                        <a href="#">Can anyone use the electronic cigarette?</a></h3>
                    <p>
                        No, the electronic cigarette is intended for use only by adults of legal age and
                        is not intended to be used by women who are pregnant or nursing and by those sensitive
                        to nicotine.</p>
                    <h3>
                        <a href="#">Can electronic cigarettes be used to quit smoking?</a></h3>
                    <p>
                        While we have had customers claim this fact, our electronic cigarettes have not
                        been tested or approved at this time as a smoking cessation device.</p>
                    <h3>
                        <a href="#">What is a nicotine cartridge?</a></h3>
                    <p>
                        The orange end of your electronic cigarette is the disposable and replaceable nicotine
                        cartridge that contains the nicotine liquid that is atomized when inhaled.</p>
                    <h3>
                        <a href="#">How do you change cartridges?</a></h3>
                    <p>
                        To change the nicotine cartridge, simply unscrew the existing cartridge (orange
                        end) and reverse the step with a new replacement.</p>
                    <h3>
                        <a href="#">What if my battery is dead?</a></h3>
                    <p>
                        If you're on a monthly plan with us, we offer free battery replacements. Just call
                        our customer service to get your exchange setup. If you aren't on a monthly plan,
                        we offer replacement batteries at a nominal cost. Please contact our customer service
                        to purchase.</p>
                    <h3>
                        <a href="#">How long can does a single cartridge last?</a></h3>
                    <p>
                        A nicotine cartridge lasts several hours to a few days depending on your smoking
                        frequency. With regular use, one cartridge is equivalent to about two packs of traditional
                        cigarettes.</p>
                    <h3>
                        <a href="#">Are there any other nicotine cartridge options?</a>
                    </h3>
                    <p>
                        Your starter kit comes with nicotine in high strength, but we also offer medium
                        strength and low strength. Our cartridges also come in menthol. Call us to switch
                        or to order more.
                    </p>
                    <h3>
                        <a href="#">What is the warranty on my electronic cigarette?</a></h3>
                    <p>
                        Our electronic cigarette comes with a one-year limited warranty. Should your unit
                        become unusable from normal use, we will replace it for free. However, if you have
                        an active refill membership, your unit is covered for replacement at any time. Please
                        contact our customer service for details.</p>
                    <h3>
                        <a href="#">How do I return a product?</a></h3>
                    <p>
                        First contact our customer service number at the top of this page to request an
                        RMA (Return Merchandise Authorization). It's important to write this number on your
                        return package. Return shipping costs are the sole responsibility of the customer.
                        Send your package to:<br>
                        <br>
                        <strong>USA Warehouse Return Address:</strong><br>
                        E-Cigs Returns Department<br>
												1091 Centre Road, Suite #100<br>
												Auburn Hills, Michigan 48326<br>
                        <br>
                        <strong>UK Warehouse Return Address:</strong><br>
                        E-Cigs Returns Department<br>
                        4 The Fort,<br>
                        Walthew House Lane,<br>
                        Martland Park,<br>
                        Wigan, WN5 0BY
                    </p>
                </div>
        </div>
        <div class="right">
            <uc:Accessories ID="cAccessories" runat="server" />
        </div>
        <div style="clear: both;">
        </div>
    </div>
</asp:Content>
